using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;


// Movement Type Enum
// --------------------
public enum MovementType
{
    WorldPos,
    WorldPosTrackCursor,
    TowardsCursor
}


public class PlayerController : MonoBehaviour
{
    // Variables
    // --------------------
    [SerializeField] private Camera _playerCam;
    private CharacterController _charCon;

    public bool InControl = true;

    [Header("Movement & Looking")]
    public float Speed = 10.0f;
    public MovementType MoveType = MovementType.WorldPos;
    //public bool LookCursor = false;
    //public bool WorldSpaceMove = true;
    private Vector3 _movementDir;
    private Vector3 _skewedMoveDir;

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
    }

    private void Update()
    {
        if (InControl)
        {
            MovementManager();
        }
        Gravity();
    }


    // Created Methods
    // --------------------
    private void MoveToWorldPos()
    {
        // Rotating Axis to Up
        var matrixRot = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        _skewedMoveDir = matrixRot.MultiplyPoint3x4(_movementDir).normalized;


        // Move, Normalise and make Vector proportional to the Speed per second.
        _charCon.Move(_skewedMoveDir * Speed * Time.deltaTime);
    }

    private void RotateToCursor()
    {
        //player looks at the mouse LookCursor position
        Ray ray = _playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }

    void MovementManager()
    {
        // Get the horizontal and vertical input.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Set the movement vector based on the axis input.
        _movementDir.Set(h, 0f, v);


        if (MoveType == MovementType.WorldPos) // Moves Player With World's Axis & Rotates Towards Move Direction
        {
            MoveToWorldPos();

            //rotate the game object with the direction of the movement
            if (_movementDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_skewedMoveDir);
            }
        }
        else if (MoveType == MovementType.WorldPosTrackCursor) // Moves Player With World's Axis & Rotates Towards Cursor Position
        {
            MoveToWorldPos();
            RotateToCursor();
        }
        else if (MoveType == MovementType.TowardsCursor) // Rotates Player Towards Cursor Position & Moves Relative To It's Position (Follows Cursor)
        {
            RotateToCursor();

            if (_movementDir.magnitude != 0)
            {
                // Move Player Towards Look Rotation
                var matrixRot = Matrix4x4.Rotate(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
                _skewedMoveDir = matrixRot.MultiplyPoint3x4(_movementDir).normalized;
                _charCon.Move(_skewedMoveDir * Speed * Time.deltaTime);
            }
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