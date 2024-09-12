using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    private List<Collider2D>enemies;
    private bool isMoving;
    private Vector2 input;
    [SerializeField]
    private Vector3 aimPosition;
    
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>(); 
        aimPosition = transform.position;
        aimPosition.x += 0.32f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(input != Vector2.zero) {
                var targetPos = transform.position;
                targetPos.x += input.x * .32f;
                targetPos.y += input.y * .32f;

                if(isWalkable(targetPos)) {
                    StartCoroutine(Move(targetPos));
                    aimPosition = targetPos;
                    aimPosition.x += input.x * .32f;
                    aimPosition.y += input.y * .32f;
                }
            }
        }
        animator.SetFloat("speed", Mathf.Abs(input.x)+Mathf.Abs(input.y)); // if in movement it will play anomation

        if(Input.GetKeyDown("space")) {
            baseAttack(aimPosition, 5);
        }
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
}
