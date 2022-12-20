using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


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

    public bool InControl = true;

    [Header("Movement & Looking")]
    public float Speed = 10.0f;
    public MovementType MoveType = MovementType.WorldPos;
    [Range(0,359)]
    [SerializeField] int _controlsSkewAngle = 45;
    private Matrix4x4 _matrixRot;
    private Vector3 _movementDir;
    private Vector3 _skewedMoveDir;

    [Header("Dodge")]
    public DodgeType dashType = DodgeType.ToMoveDirection;
    [SerializeField] GameObject _dodgeEffect;
    [SerializeField] float dodgeCooldown = 2f;
    private float _dodgeCooldownTimer;
    [SerializeField] float dodgeDuration = 0.2f;
    private float dodgeDurationTimer;
    [SerializeField] float dodgeSpeed = 50;
    private Vector3 _cachedSkewedDir;
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
    }

    private void Update()
    {
        if (InControl)
        {
            MovementManager();
            DodgeManager();
            Gravity();
        }
    }


    private void SubscribeToInputs()
    {
        _plInputActions.Player.Dodge.performed += Dodge;
    }


    public void IncreaseMaxSpeed(float speedIncrease)
    {
        Speed += speedIncrease;
    }

    // Created Methods
    // --------------------
    private void MoveToWorldPos()
    {
        // Rotating Axis to Up
        _skewedMoveDir = _matrixRot.MultiplyPoint3x4(_movementDir).normalized;


        // Move, Normalise and make Vector proportional to the Speed per second.
        _charCon.Move(_skewedMoveDir * Speed * Time.deltaTime);
    }

    private void RotateToCursor()
    {
        //player looks at the mouse LookCursor position
        //Ray ray = _playerCam.ScreenPointToRay(Input.mousePosition);

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
            }
        }
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

        //Gets a direction for the dodge when player stands still
        if (_movementDir.magnitude != 0)
        {
            _cachedSkewedDir = _skewedMoveDir;
        }

        // Dodging Code
        if (_canDodge && _calledDodge)
        {
            dodgeDurationTimer += Time.deltaTime;
            Vector3 dodgeDir;
            if (dashType == DodgeType.ToMoveDirection) //Sets Dodge direction to movement direction
            {
                if (_cachedSkewedDir != Vector3.zero)
                {
                    dodgeDir = _cachedSkewedDir;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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