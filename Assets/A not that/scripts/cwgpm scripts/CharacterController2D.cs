using System.Collections;
using UnityEngine;


public class CharacterController2D : MonoBehaviour
{
        public float speed = 1;
        public float acceleration = 2;

         //public Vector3 nextMoveCommand;
         //added
        public Vector3 change;
         // ^added
        public Animator animator;
        public bool flipX = false;
        
        public FloatValue currentHealth;
        public Signal playerHealthSignal;
        public float knockTime;
        public Inventory playerInventory;
        public SpriteRenderer receivedItemSprite;
        public VectorValue startingPosition;


        //

        private Rigidbody2D myRigidbody;
        SpriteRenderer spriteRenderer;
      //  PixelPerfectCamera pixelPerfectCamera;

        enum State
        {
            Idle, Moving, Interact, Sitting
        }

        State state = State.Idle;
      //  Vector3 start, end;
    //    Vector2 currentVelocity;
     //   float startTime;
     //   float distance;
     //   float velocity;

    void IdleState()
      { animator.SetBool("moving", false);
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if (change != Vector3.zero)
        {
            playerHealthSignal.Raise();
            animator.SetBool("moving", true);
            state = State.Moving;

            /*
            start = transform.position;
            end = start + nextMoveCommand;
            distance = (end - start).magnitude;
            velocity = 0;

            UpdateAnimator(nextMoveCommand);

            nextMoveCommand = Vector3.zero;

                state = State.Moving;
                */
        }
    }

    void MoveState()
    {
        animator.SetBool("moving", true);
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("WalkX", change.x < 0 ? -1 : change.x > 0 ? 1 : 0);
        animator.SetFloat("WalkY", change.y < 0 ? 1 : change.y > 0 ? -1 : 0);
        myRigidbody.MovePosition(transform.position + change.normalized * speed * Time.deltaTime);
            if (change == Vector3.zero)
            {
                 state = State.Idle;
             }

        }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Moving:
                MoveState();
                break;
            case State.Sitting:
                SittingAnimate();
                break;
            case State.Interact:
                Interact();
                break;

        }
            //Debug.Log("animator.IsSitting = " + animator.GetBool("IsSitting"));
            //animator.SetBool("IsSitting", true);
        }



      /* void LateUpdate()
        {
            if (pixelPerfectCamera != null)
            {
                transform.position = pixelPerfectCamera.RoundToPixel(transform.position);
            }
        }
      */
    void Awake()
    {
 
        Debug.Log("Awake");
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = startingPosition.initialValue;
    //  pixelPerfectCamera = GameObject.FindObjectOfType<PixelPerfectCamera>();
            
    }


    public void RaiseItem()
    {
        if (playerInventory.currentItem != null)
        {
            if (state != State.Interact)
            {
                // there's an issue with leaving the interact state here
                animator.SetBool("ReceiveItem", true);
                state = State.Interact;
                //animator.speed = 0; no , adding change instead
                change = Vector3.zero;
                // ADD change ZERO !!
                //  myRigidbody.velocity = Vector2.zero;
                receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
            }
            else
            {
                animator.SetBool("ReceiveItem", false);
                state = State.Idle;
                receivedItemSprite.sprite = null;
                playerInventory.currentItem = null;
                //maybe remove older line below
                animator.speed = 1;
                //maybe remove line above 
            }
        }
     }

    public void SittingAnimate()
    {
        // speed = 0; is bad
        // adding line below instead
        change = Vector3.zero;
        //just added the line above
        // myRigidbody.velocity = Vector2.zero;
        animator.SetBool("IsSitting", true);
        //
        Vector3 sittingPosition1 = new Vector3(-5, 4, 0); 
        gameObject.transform.position = sittingPosition1;
        //
        Debug.Log("should be animating a sitting animation");
    }

    public void SittingState()
    {
        if (animator.GetBool("IsSitting") == false)
        {
            state = State.Sitting;
            animator.SetBool("IsSitting", true);
        }
        else
        {
            state = State.Idle;
            Vector3 standingPosition1 = new Vector3(-5, 3, 0);
            gameObject.transform.position = standingPosition1;
            animator.SetBool("IsSitting", false);
            //adding line below (didn't do anything)
            //change = Vector3.zero;
            //adding line above
        }
    }

    public void Interact()
    {
        Debug.Log("interact state");
        change = Vector3.zero;
        // ADD MOVEMENT ZERO
        // myRigidbody.velocity = Vector2.zero;

    }

    public void Knock(float knockTime, float damage)
    {
        playerHealthSignal.Raise();
        currentHealth.RuntimeValue -= damage;
        if (currentHealth.RuntimeValue > 0)
        {
            StartCoroutine(KnockCo(knockTime, damage));
        }
        else
        {
            Debug.Log("no cats");
        }
    }

    private IEnumerator KnockCo(float knockTime, float damage)
    {
        {
            yield return new
            WaitForSeconds(knockTime);
            if (state != State.Interact)
            {
                state = State.Idle;
            }
        }
    }
}