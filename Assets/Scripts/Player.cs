using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public GameManager gameManager;

    [Header("Movement")]
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    private bool isMoving;
    private Vector2 input;
    [SerializeField]
    private Vector3 aimPosition;

    [Header("Combat")]
    public float maxHealth;
    private float currentHealth;

    
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>(); 
        aimPosition = transform.position;
        aimPosition.x += 0.32f;

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift)) {
            if(Input.GetKeyDown("w")) {
                input.y = .32f;
                aimPosition.y = transform.position.y + input.y;
                aimPosition.x = transform.position.x;
            } if(Input.GetKeyDown("a")) {
                input.x = -.32f;
                aimPosition.x = transform.position.x + input.x;
                aimPosition.y = transform.position.y;
            } if(Input.GetKeyDown("s")) {
                input.y = -.32f;
                aimPosition.y = transform.position.y + input.y;
                aimPosition.x = transform.position.x;
            } if(Input.GetKeyDown("d")) {
                input.x = .32f;
                aimPosition.x = transform.position.x + input.x;
                aimPosition.y = transform.position.y;
            }
            
            
        }
        else if(!isMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(input != Vector2.zero) {
                var targetPos = transform.position;
                targetPos.x += input.x * .32f;
                targetPos.y += input.y * .32f;

                aimPosition = targetPos;
                aimPosition.x += input.x * .32f;
                aimPosition.y += input.y * .32f;

                if(isWalkable(targetPos)) {
                    StartCoroutine(Move(targetPos));
                    
                    endTurn();
                } else {
                    aimPosition.x -= input.x * .32f;
                    aimPosition.y -= input.y * .32f;
                }
            }
            if(Input.GetKeyDown("space")) {
                StartCoroutine(attackMovement(aimPosition));
                baseAttack(aimPosition, 5);
                endTurn();
            }
        }
        animator.SetFloat("speed", Mathf.Abs(input.x)+Mathf.Abs(input.y)); // if in movement it will play animation

        
    }

    IEnumerator Move(Vector3 targetPos) {
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    private bool isWalkable(Vector3 targetPos) {
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectLayer | interactableLayer) != null) {
            return false;
        }
        return true;
    }

    void baseAttack(Vector3 aimPosition, float dmg) {
        if(Physics2D.OverlapCircleAll(aimPosition, 0.1f, interactableLayer).Count() != 0) {
            Physics2D.OverlapCircleAll(aimPosition, 0.1f, interactableLayer)[0].GetComponent<Enemy>().takeDamage(dmg);
        }
    }

    IEnumerator attackMovement(Vector3 aimPosition) {
        isMoving = true;
        var startPosition = transform.position;
        var aimPos = transform.position + (aimPosition-transform.position)*.5f;
        bool isForward = true;
        while((aimPos - transform.position).sqrMagnitude > Mathf.Epsilon && isForward) {
            transform.position = Vector3.MoveTowards(transform.position, aimPos, 2*moveSpeed * Time.deltaTime);
            yield return null;
        }
        isForward = false;
        while((startPosition - transform.position).sqrMagnitude > Mathf.Epsilon && !isForward) {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, 2*moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPosition;
        isMoving = false;
    }

    private void endTurn() {
        gameManager.GetComponent<GameManager>().enemiesTurn();
    }
}
