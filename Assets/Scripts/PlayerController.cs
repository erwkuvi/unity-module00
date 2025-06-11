using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public InputActionAsset actionAsset;
    
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private bool _isGrounded;
    private bool _floorIsLava = false; 
    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private bool _jumpQueued = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _isGrounded = true;
    }

    private void OnEnable()
    {
        _jumpAction = actionAsset.FindActionMap("Player").FindAction("Jump");
        _moveAction = actionAsset.FindActionMap("Player").FindAction("Move");
        _jumpAction.Enable();
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _jumpAction.Disable();
        _moveAction.Disable();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Queue jump if triggered
        if (_jumpAction.triggered && _isGrounded && !_floorIsLava)
        {
            _jumpQueued = true;
        }
    }

    void FixedUpdate()
    {
        if (_floorIsLava) return;

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _moveDirection = new Vector3(-moveInput.y, 0, moveInput.x);

        //_rb.MovePosition(_rb.position + moveDirection * speed * Time.fixedDeltaTime);
        _rb.AddForce(_moveDirection * speed);

        if (_jumpQueued)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _isGrounded = false;
            _jumpQueued = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _isGrounded = true;

        if (other.collider.tag == "Lava")
        {
            _floorIsLava = true; 
            Debug.Log("GAME OVER");
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            Destroy(this.gameObject);
            return;
        }
        if (other.collider.tag == "Finish")
        {
            _floorIsLava = true; 
            Debug.Log("FINISH");
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
    }
}