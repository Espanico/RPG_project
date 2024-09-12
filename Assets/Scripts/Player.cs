using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    private bool isMoving;
    private Vector2 input;
    private Vector3 moveDelta;
    
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>(); 
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
                }
            }
        }
        animator.SetFloat("speed", Mathf.Abs(input.x)+Mathf.Abs(input.y));
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
        if(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectLayer) != null) {
            return false;
        }
        return true;
    }
}
