using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("플레이어 속성")]
    public float speed = 5;
    public float jumpUp = 1;
    public float power = 5;
    public Vector3 direction;
    public GameObject slash;

    //그림자
    public GameObject Shadow1;
    List<GameObject> sh = new List<GameObject>();

    //히트 이펙트
    public GameObject hit_lazer;




    bool bJump = false;
    Animator pAnimator;
    Rigidbody2D pRig2D;
    SpriteRenderer sp;

    public GameObject Jdust;


    //벽점프
    public Transform wallChk;
    public float wallchkDistance;
    public LayerMask wLayer;
    bool isWall;
    public float slidingSpeed;
    public float wallJumpPower;
    public bool isWallJump;
    float isRight = 1;


    public GameObject walldust;


    void Start()
    {
        pAnimator = GetComponent<Animator>();
        pRig2D = GetComponent<Rigidbody2D>();
        direction = Vector2.zero;
        sp = GetComponent<SpriteRenderer>();

    }


    void KeyInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal"); //왼쪽은 -1   0   1

        if(direction.x <0)
        {
            //left
            sp.flipX = true;
            pAnimator.SetBool("Run", true);

            //점프벽잡기 방향
            isRight = -1;


            //Shadowflip
            for(int i =0; i<sh.Count; i++)
            {
                sh[i].GetComponent<SpriteRenderer>().flipX = sp.flipX;
            }

        }
        else if(direction.x >0)
        {
            //right
            sp.flipX = false;
            pAnimator.SetBool("Run", true);

            isRight = 1;

            //Shadowflip
            for (int i = 0; i < sh.Count; i++)
            {
                sh[i].GetComponent<SpriteRenderer>().flipX = sp.flipX;
            }


        }
        else if(direction.x == 0)
        {
            pAnimator.SetBool("Run", false);


            for (int i = 0; i < sh.Count; i++)
            {
                Destroy(sh[i]); //게임오브젝트지우기
                sh.RemoveAt(i); //게임오브젝트 관리하는 리스트지우기
            }





        }


        if (Input.GetMouseButtonDown(0)) //0번 왼쪽마우스
        {
            pAnimator.SetTrigger("Attack");
            Instantiate(hit_lazer, transform.position, Quaternion.identity);

        }




    }

    
    void Update()
    {

        if(!isWallJump)
        {
            KeyInput();
            Move();
        }
       


        //벽인지 체크
        isWall = Physics2D.Raycast(wallChk.position, Vector2.right * isRight, wallchkDistance, wLayer);
        pAnimator.SetBool("Grab", isWall);





        if(Input.GetKeyDown(KeyCode.W))
        {
            if(pAnimator.GetBool("Jump")==false)
            {
                Jump();
                pAnimator.SetBool("Jump", true);
                JumpDust();
            }
          
        }



        if(isWall)
        {
            isWallJump = false;
            //벽점프상태
            pRig2D.linearVelocity = new Vector2(pRig2D.linearVelocityX, pRig2D.linearVelocityY * slidingSpeed);
            //벽을 잡고있는 상태에서 점프
            if(Input.GetKeyDown(KeyCode.W))
            {
                isWallJump = true;
                //벽점프 먼지
                GameObject go = Instantiate(walldust, transform.position + new Vector3(0.8f * isRight, 0, 0), Quaternion.identity);
                go.GetComponent<SpriteRenderer>().flipX = sp.flipX;

                Invoke("FreezeX", 0.3f);
                //물리
                pRig2D.linearVelocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);

                sp.flipX = sp.flipX == false ? true : false;
                isRight = -isRight;
                
            }

        }

    }


    void FreezeX()
    {
        isWallJump = false;
    }







    private const float GROUND_CHECK_DISTANCE = 0.7f;






    private void FixedUpdate()
    {
        Debug.DrawRay(pRig2D.position, Vector3.down, new Color(0, GROUND_CHECK_DISTANCE, 0));

        //레이캐스트로 땅체크 
        RaycastHit2D rayHit = Physics2D.Raycast(pRig2D.position, Vector3.down, GROUND_CHECK_DISTANCE, LayerMask.GetMask("Ground"));

        CheckGroundedState(rayHit);
    }


    void CheckGroundedState(RaycastHit2D rayHit)
    {

        bool isGrounded = rayHit.collider != null && rayHit.distance < GROUND_CHECK_DISTANCE;
       
        if (isGrounded)
        {
                pAnimator.SetBool("Jump", false);                
        }
        else
        {
            //떨어지고 있다
            if (!isWall)
            {
                //그냥 떨어지는중
                pAnimator.SetBool("Jump", true);
            }
            else
            {
                //벽타기
                pAnimator.SetBool("Grab", true);
            }
        }   
        
    }










    public void Jump()
    {
        pRig2D.linearVelocity = Vector2.zero;

        pRig2D.AddForce(new Vector2(0, jumpUp), ForceMode2D.Impulse);
    }





    public void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }



    public void AttSlash()
    {
        //플레이어 오른쪽
        if(sp.flipX == false)
        {
            pRig2D.AddForce(Vector2.right * power, ForceMode2D.Impulse);
            //플레이어 오른쪽
            GameObject go = Instantiate(slash, transform.position, Quaternion.identity);
            //go.GetComponent<SpriteRenderer>().flipX = sp.flipX;
        }
        else
        {

            pRig2D.AddForce(Vector2.left * power, ForceMode2D.Impulse);
            //왼쪽
            GameObject go = Instantiate(slash, transform.position, Quaternion.identity);
            //go.GetComponent<SpriteRenderer>().flipX = sp.flipX;
        }   

    }

    //그림자
    public void RunShadow()
    {
        if(sh.Count<6)
        {
            GameObject go = Instantiate(Shadow1, transform.position, Quaternion.identity);
            go.GetComponent<Shadow>().TwSpeed = 10 - sh.Count; 
            sh.Add(go);
        }
    }



    //흙먼지
    public void RandDust(GameObject dust)
    {



        Instantiate(dust, transform.position +new Vector3(-0.114f,-0.467f,0), Quaternion.identity);




    }

    //점프먼지
    public void JumpDust()
    {
        if(!isWall)
        {
            Instantiate(Jdust, transform.position, Quaternion.identity);
            Debug.Log("점프먼지 생성중이야");
        }
        else
        {
            //벽먼지
            //Instantiate(walldust, transform.position, Quaternion.identity);
            //Debug.Log("나벽먼지 생성중이야");
        }

      
    }

    //벽점프
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallChk.position, Vector2.right * isRight * wallchkDistance);
    }




}
