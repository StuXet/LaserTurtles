using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Movement Type Enum
// --------------------
public enum MovementType
{
    WorldPos,
    WorldPosTrackLook,
}

// Dodge Type Enum
// --------------------
public enum DodgeType
{
    ToMoveDirection,
    TowardsLook
}

public class PlayerController : MonoBehaviour
{
    // Variables
    // --------------------
    [SerializeField] private InputManager _inputManagerRef;
    private PlayerInputActions _plInputActions;

    [SerializeField] private Camera _playerCam;
    private HealthHandler _healthHandlerRef;
    private CharacterController _charCon;

    [SerializeField] private Animator _playerAnimator;

    public bool InControl = true;
    private bool _isDead = false;

    [Header("Movement & Looking")]
    public float MaxSpeed = 10.0f;
    private float _currentSpeed;
    [SerializeField] private float _acceleration = 40;
    [SerializeField] private float _deceleration = 40;
    public MovementType MoveType = MovementType.WorldPos;
    [Range(0, 359)]
    [SerializeField] int _controlsSkewAngle = 45;
    private Matrix4x4 _matrixRot;
    private Vector3 _movementDir;
    private Vector3 _skewedMoveDir;
    private Vector3 _lastSkewedMoveDir;
    private Vector3 _mousePosDelta;
    private int _mouseAngleDelta;
    private int _moveAngleDelta;
    private int _mouseMoveAngleDelta;

    [Header("Dodge")]
    public DodgeType dashType = DodgeType.ToMoveDirection;
    [SerializeField] GameObject _dodgeEffect;
    [SerializeField] float dodgeCooldown = 2f;
    private float _dodgeCooldownTimer;
    [SerializeField] float dodgeDuration = 0.2f;
    private float dodgeDurationTimer;
    [SerializeField] float dodgeSpeed = 50;
    [HideInInspector] public bool isDodging;
    private bool _canDodge;
    private bool _calledDodge;

    [Header("Gravity")]
    public LayerMask GroundMask;
    public Transform GroundCheck;
    public float GroundDistance = 0.5f;
    public float GravityModifier = 10f;
    private Vector3 _velocity;
    private bool _isGrounded;
    public bool GravityEnabled = true;

    public bool IsDead { get => _isDead;}


    // Default Methods
    // --------------------
    private void Awake()
    {
        _charCon = GetComponent<CharacterController>();
        _healthHandlerRef = GetComponent<HealthHandler>();
        _healthHandlerRef.OnDeathOccured += _healthHandlerRef_OnDeathOccured;
    }

    private void Start()
    {
        _plInputActions = _inputManagerRef.PlInputActions;
        SubscribeToInputs();

        _matrixRot = Matrix4x4.Rotate(Quaternion.Euler(0, _controlsSkewAngle, 0));
        _dodgeCooldownTimer = dodgeCooldown;

        int option = PlayerPrefs.GetInt("CursorLook");
        if (option == 0)
        {
            MoveType = MovementType.WorldPos;
        }
        else
        {
            MoveType = MovementType.WorldPosTrackLook;
        }
    }

    private void Update()
    {
        if (InControl)
        {
            MovementManager();
            DodgeManager();
            Gravity();
        }
        AnimationHandler();
    }


    private void SubscribeToInputs()
    {
        _plInputActions.Player.Dodge.performed += Dodge;
    }


    public void IncreaseMaxSpeed(float speedIncrease)
    {
        MaxSpeed += speedIncrease;
    }

    // Created Methods
    // --------------------
    private void MoveToWorldPos()
    {
        // Rotating Axis to Up
        _skewedMoveDir = _matrixRot.MultiplyPoint3x4(_movementDir).normalized;

        if (_movementDir != Vector3.zero)
        {
            _lastSkewedMoveDir = _skewedMoveDir;
        }

        // Move, Normalise and make Vector proportional to the Speed per second.

        if (_movementDir != Vector3.zero)
        {
            if (_currentSpeed < MaxSpeed)
            {
                _currentSpeed += _acceleration * Time.deltaTime;
            }
            else if (_currentSpeed >= MaxSpeed)
            {
                _currentSpeed = MaxSpeed;
            }
        }
        else
        {
            if (_currentSpeed > 0)
            {
                _currentSpeed -= _deceleration * Time.deltaTime;
            }
            else
            {
                _currentSpeed = 0;
            }
        }

        _charCon.Move(_lastSkewedMoveDir * _currentSpeed * Time.deltaTime);

        _moveAngleDelta = (int)(Mathf.Atan2(_lastSkewedMoveDir.x, _lastSkewedMoveDir.z) * Mathf.Rad2Deg) + 180;
    }

    private void RotateToCursor()
    {
        //player looks at the mouse LookCursor position

        if (_plInputActions.Player.MouseMovement.IsInProgress())
        {
            Vector2 mousePos = _plInputActions.Player.MouseLook.ReadValue<Vector2>();
            Ray ray = _playerCam.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 target = hit.point;
                target.y = transform.position.y;
                transform.LookAt(target);
            }

            _mousePosDelta = new Vector3((mousePos.x - Screen.width / 2) / (Screen.width / 2), 0, (mousePos.y - Screen.height / 2) / (Screen.height / 2)).normalized;
            _mousePosDelta = _matrixRot.MultiplyPoint3x4(_mousePosDelta);
            _mouseAngleDelta = (int)(Mathf.Atan2(_mousePosDelta.x, _mousePosDelta.z) * Mathf.Rad2Deg) + 180;
        }
        else
        {
            Vector2 stickVec = _plInputActions.Player.StickLook.ReadValue<Vector2>();
            Vector3 tempRot = new Vector3(stickVec.x, 0, stickVec.y);
            Vector3 delta = _matrixRot.MultiplyPoint3x4(tempRot).normalized;
            if (delta.magnitude != 0)
            {
                float angles = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, angles, 0);

                _mouseAngleDelta = (int)angles + 180;
            }
        }

        _mouseMoveAngleDelta = _mouseAngleDelta - _moveAngleDelta;
    }

    void MovementManager()
    {
        // Get the horizontal and vertical input.
        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        // Set the movement vector based on the axis input.
        //_movementDir.Set(h, 0, v);
        _movementDir.Set(_plInputActions.Player.Move.ReadValue<Vector2>().x, 0, _plInputActions.Player.Move.ReadValue<Vector2>().y);

        if (MoveType == MovementType.WorldPos) // Moves Player With World's Axis & Rotates Towards Move Direction
        {
            MoveToWorldPos();

            //rotate the game object with the direction of the movement
            if (_movementDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_skewedMoveDir);
            }
        }
        else if (MoveType == MovementType.WorldPosTrackLook) // Moves Player With World's Axis & Rotates Towards Cursor Position
        {
            MoveToWorldPos();
            RotateToCursor();
        }
    }



    private void Dodge(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        _calledDodge = true;
    }

    private void DodgeManager()
    {
        // Dodge Cooldown Timer
        if (_dodgeCooldownTimer >= dodgeCooldown)
        {
            _canDodge = true;
        }
        else
        {
            _canDodge = false;
            _calledDodge = false;
            _dodgeCooldownTimer += Time.deltaTime;
        }

        // Dodging Code
        if (_canDodge && _calledDodge)
        {
            dodgeDurationTimer += Time.deltaTime;
            Vector3 dodgeDir;
            if (dashType == DodgeType.ToMoveDirection) //Sets Dodge direction to movement direction
            {
                if (_lastSkewedMoveDir != Vector3.zero)
                {
                    dodgeDir = _lastSkewedMoveDir;
                }
                else
                {
                    dodgeDir = new Vector3(1, 0, -1);//Direction of dodge when player haven't pressed any keys
                }
            }
            else
            {
                dodgeDir = transform.forward;
            }
            if (dodgeDurationTimer <= dodgeDuration) //Sets Dodge direction to look/cursor direction
            {
                _charCon.Move(dodgeDir * dodgeSpeed * Time.deltaTime);
                isDodging = true;
            }
            else
            {
                // Reset Values 
                isDodging = false;
                _calledDodge = false;
                _dodgeCooldownTimer = 0;
                dodgeDurationTimer = 0;
            }
        }

        DodgeEffect();
    }

    private void DodgeEffect()
    {
        if (isDodging)
        {
            _dodgeEffect.SetActive(true);
        }
        else
        {
            _dodgeEffect.SetActive(false);
        }
    }


    private void Gravity()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (GravityEnabled)
        {

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2;
            }


            _velocity.y -= GravityModifier * Time.deltaTime;
            _charCon.Move(_velocity * Time.deltaTime);
        }
    }


    private void _healthHandlerRef_OnDeathOccured(object sender, System.EventArgs e)
    {
        PlayerDeath();
    }

    private void PlayerDeath()
    {
        _isDead = true;
        InControl = false;
        StartCoroutine(DeathDelay());
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void AnimationHandler()
    {
        if (_playerAnimator)
        {
            // Idle & Movement
            _playerAnimator.SetFloat("Speed", _currentSpeed / MaxSpeed);

            // Move Direction
            if (MoveType == MovementType.WorldPos)
            {
                _playerAnimator.SetFloat("MoveX", 0);
                _playerAnimator.SetFloat("MoveZ", 1);
            }
            else
            {
                if ((_mouseMoveAngleDelta <= 45 && _mouseMoveAngleDelta >= -45))
                {
                    _playerAnimator.SetFloat("MoveZ", 1);
                }
                else if (_mouseMoveAngleDelta <= 225 && _mouseMoveAngleDelta >= 135 || _mouseMoveAngleDelta <= -135 && _mouseMoveAngleDelta >= -225)
                {
                    _playerAnimator.SetFloat("MoveZ", -1);
                }
                //else
                //{
                //    _playerAnimator.SetFloat("MoveZ", 0);
                //}

                //if (_mouseMoveAngleDelta < -45 && _mouseMoveAngleDelta > -135)
                //{
                //    _playerAnimator.SetFloat("MoveX", 1);
                //}
                //else if (_mouseMoveAngleDelta < 135 && _mouseMoveAngleDelta > 45)
                //{
                //    _playerAnimator.SetFloat("MoveX", -1);
                //}
                //else
                //{
                //    _playerAnimator.SetFloat("MoveX", 0);
                //}
            }


            // Dodge
            _playerAnimator.SetBool("Dodge", _calledDodge);

            // Death
            if (_isDead)
            {
                _playerAnimator.SetTrigger("Death");
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(GroundCheck.position, GroundDistance);
    }
}