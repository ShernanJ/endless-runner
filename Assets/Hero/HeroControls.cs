using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroControls : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float gravity = -50f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private float moveInput = 1;
    [SerializeField] private Transform groundCollision;
    [SerializeField] private Transform[] wallCollision;
    [SerializeField] private Transform ceilingCollision;
    [SerializeField] private float slideTime = 0.5f;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip slideSound;
    [SerializeField] private AudioClip blockedSound;
    [SerializeField] private AudioClip fallingSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioSource runSound;


    private CharacterController _controller;
    private Animator _animator;

    private Vector3 move;
    private bool isGrounded;
    private bool isBlocked;
    private bool isSliding;
    private bool isFalling;
    private bool isUnderCeiling;
    private bool isOnLedge;
    private float slideTimeCounter;
    private float fallTimeCounter;
    private float ledgeTimeCounter;
    private bool isMoving;
    private bool jumpPressed;
    private float jumpTimer;
    private float jumpPeriod = 0.2f;
    private bool blockAudioPlayed = false;
    private bool landAudioPlayed = true;
    private bool fallAudioPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }
        slideTimeCounter -= Time.deltaTime;
        ledgeTimeCounter -= Time.deltaTime;
        transform.forward = new Vector3(moveInput, 0, Mathf.Abs(moveInput) - 1);

        if (slideTimeCounter < 0)
        {
            isSliding = false;
        }

        if (fallTimeCounter > 0.8)
        {
            isFalling = true;
        }
        if (ledgeTimeCounter < 0)
        {
            isOnLedge = false;
        }

        // Check if player is grounded
        isGrounded = false;

        if (Physics.CheckSphere(groundCollision.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
        }

        if (isGrounded && move.y < 0)
        {
            move.y = 0;
            SoundManager.Instance.ChangeMusicUnderCeiling(false);
            if (!landAudioPlayed)
            {
                SoundManager.Instance.PlaySound(landingSound);
                landAudioPlayed = true;
            }
        }
        else
        {
            if(!isOnLedge)
            {
                move.y += gravity * Time.deltaTime;
            }
        }
        if(isGrounded)
        {
            SoundManager.Instance.StopFalling(fallingSound);
        }
        if (!isGrounded && !isOnLedge)
        {
            fallTimeCounter += Time.deltaTime;
            if (move.y < -5)
            {
                SoundManager.Instance.ChangeMusicUnderCeiling(true);
                if (!fallAudioPlayed && isFalling)
                {
                    SoundManager.Instance.PlayFalling(fallingSound);
                    fallAudioPlayed = true;
                }
            }
            landAudioPlayed = false;
        }


        // Check for Wall
        foreach (var c in wallCollision)
        {
            isBlocked = Physics.CheckSphere(c.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
            if (isBlocked)
            {
                isOnLedge = Physics.CheckSphere(groundCollision.position, 0.1f, ledgeLayer, QueryTriggerInteraction.Ignore) || Physics.CheckSphere(c.position, 0.1f, ledgeLayer, QueryTriggerInteraction.Ignore);
                if (isOnLedge && ledgeTimeCounter < 0)
                {
                    move.y = 0;
                    if (jumpPressed)
                    {
                        ledgeTimeCounter = 2f;
                    }
                    if (!blockAudioPlayed)
                    {
                        SoundManager.Instance.PlaySound(blockedSound);
                        blockAudioPlayed = true;
                    }
                    break;

                }
                
            }else
            {
                isBlocked = false;
            }
            
        }

        isUnderCeiling = Physics.CheckSphere(ceilingCollision.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore);

        if (!isBlocked && isMoving)
        {
            blockAudioPlayed = false;
            _controller.Move(new Vector3(moveInput * moveSpeed, 0, 0) * Time.deltaTime);

            if (!isSliding && isGrounded)
            {
                runSound.enabled = true;
            } else
            {
                runSound.enabled = false;
            }
        }

        jumpPressed = Input.GetButtonDown("Jump");

        if (jumpPressed)
        {
            jumpTimer = Time.time;
            isOnLedge = false;
        }

        if ((isGrounded || isOnLedge) && (jumpPressed || jumpTimer > 0 && Time.time < jumpTimer + jumpPeriod) && !isUnderCeiling && !isSliding)
        {
            move.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
            jumpTimer = -1;
            SoundManager.Instance.PlaySound(jumpSound);
        }

        if(isGrounded && !isBlocked && Input.GetButtonDown("Slide") && slideTimeCounter < 0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            SoundManager.Instance.PlaySound(slideSound);
        }

        if(Input.GetButtonDown("Stop") && isGrounded && !isBlocked && !isUnderCeiling)
        {
            if (!isSliding)
            {
                isMoving = false;
            } 
            else
            {
                isSliding = false;
            }
        }
        if (Input.GetButtonDown("Forward") && isGrounded && !isBlocked && !isUnderCeiling)
        {
            if (!isSliding)
            {
                isMoving = true;
            }
            else
            {
                isSliding = false;
            }
        }

        if (isSliding)
        {
            if(isUnderCeiling || !isGrounded){
                slideTimeCounter = slideTime;
            }
            _controller.height = 0.3f;
            _controller.center = new Vector3(0f, 0.45f, 3.2f);
            _controller.radius = 0.2f;
            foreach (var c in wallCollision)
            {
                c.localScale = new Vector3(1.0f, 0.5f, 0.1f);
                c.localPosition = new Vector3(0f, 0.3f, 4.473f);
            }
            ceilingCollision.localPosition = new Vector3(0, 2.11f, 3f);
            groundCollision.localPosition = new Vector3(0, 0, 3.5f);
            if (isUnderCeiling)
            {
                SoundManager.Instance.ChangeMusicUnderCeiling(true);
            }
        } else
        {
            _controller.height = 2;
            _controller.center = new Vector3(0f, 1f, -0.3f);
            _controller.radius = 0.5f;
            foreach (var c in wallCollision)
            {
                c.localPosition = new Vector3(0f, 1f, 0.32f);
                c.localScale = new Vector3(1.0f, 2f, 0.1f);
            }
            ceilingCollision.localPosition = new Vector3(0, 2.11f, 0);
            groundCollision.localPosition = new Vector3(0, 0, 0);
            if(move.y > -25)
            {
                SoundManager.Instance.ChangeMusicUnderCeiling(false);
            }
        }


        _controller.Move(move * Time.deltaTime);

        _animator.SetFloat("Speed", moveInput);

        _animator.SetBool("isGrounded", isGrounded);

        _animator.SetFloat("FallingSpeed", move.y);

        _animator.SetBool("isBlocked", isBlocked);

        _animator.SetBool("isSliding", isSliding);

        _animator.SetBool("isMoving", isMoving);

        _animator.SetBool("isOnLedge", isOnLedge);
    }
}
