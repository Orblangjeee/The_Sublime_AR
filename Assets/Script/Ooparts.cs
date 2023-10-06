using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Ooparts : MonoBehaviour
{
    public Camera arCamera;

    [Header("RotateObj")]
    private Quaternion initialRot;
    private Vector3 initialScale;


    [Header("Top Ring")]
    public GameObject RingObject1; // Ray 충돌체크할 메쉬 콜라이더
    public GameObject rotationObject1; // 회전값을 일치시킬 오브젝트
    public bool isDragging1 = false; // 드래그 여부
    private float initialYRotation1; // 초기 회전값
    private Vector3 initialMousePosition1; // 초기 마우스 위치
    private float accumulatedRotation1 = 0f; // 누적 회전값

    [Header("Bottom Ring")]
    public GameObject RingObject2; // Ray 충돌체크할 메쉬 콜라이더
    //public GameObject rotationObject2; // 회전값을 일치시킬 오브젝트    
    public bool isDragging2 = false; // 드래그 여부
    private float initialYRotation2; // 초기 회전값
    private Vector3 initialMousePosition2; // 초기 마우스 위치
    private float accumulatedRotation2 = 0f; // 누적 회전값

    [Header("Core")]
    public bool isDragging3 = false;
    private float currentTime;
    public float moveStationaryTime = 0.7f;
    private bool isMoved = false;
    public GameObject oopartObj;
    public ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hitInfos = new List<ARRaycastHit>(); //충돌 물체 검출할 hits
    public TextMeshProUGUI tm;
    public float rotationSpeed = 0.01f; // 회전시킬 정도(속도)
    void Start()
    {
        // 만일 ARCamera가 비어있는 경우에는 MainCam 넣기
        if (arCamera == null) { arCamera = Camera.main; }
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭
        {
            
            //2. 마우스 클릭한 위치에서 Ray를 발사
            Vector3 mousePos = Input.mousePosition;
            //1. Ray : Mouse 클릭한 위치
            Ray ray = arCamera.ScreenPointToRay(mousePos); ; // 마우스 현 위치에서 레이 쏘기
            //2. RaycastHit : 충돌한 물체 담는 그릇
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Orig"))
                {
                    rotationObject1 = GameObject.FindWithTag("Orig");
                    
                }

                if (hit.collider.CompareTag("Logic"))
                {
                    rotationObject1 = GameObject.FindWithTag("Logic");
                    
                } 
                
                if (hit.collider.CompareTag("Wisdom"))
                {
                    rotationObject1 = GameObject.FindWithTag("Wisdom");
                    
                }
                tm.text = rotationObject1.name;
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                // 레이가 충돌한 곳 감지

                if (rotationObject1 != null)
                {
                    if (hit.collider.CompareTag("Ring1")) // 상단 링에 맞으면
                    {
                        initialScale = rotationObject1.transform.localScale;

                        // 회전값 갱신
                        isDragging1 = true;
                        initialYRotation1 = RingObject1.transform.localEulerAngles.y;
                        initialMousePosition1 = Input.mousePosition;
                        accumulatedRotation1 = 0f; // 새로 클릭할 때마다 누적 회전값 초기화

                    }

                    if (hit.collider.CompareTag("Ring2")) // 하단 링에 맞으면
                    {
                        initialRot = rotationObject1.transform.localRotation;
                        //// 회전값 갱신
                        isDragging2 = true;
                        initialYRotation2 = RingObject2.transform.localEulerAngles.y;
                        initialMousePosition2 = Input.mousePosition;
                        accumulatedRotation2 = 0f; // 새로 클릭할 때마다 누적 회전값 초기화

                    }

                    if (hit.collider.CompareTag("Core")) // 코어에 맞으면
                    {

                        //isDragging3 = true;
                        
                    }
                } else
                {
                    Debug.Log("오브젝트가 비어있습니다.");
                }
               

               
            }

        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 놓음
        {
          if (rotationObject1 != null)
            {
                isDragging1 = false;
                isDragging2 = false;
                //isDragging3 = false;
                initialScale = rotationObject1.transform.localScale;
                initialRot = rotationObject1.transform.localRotation;
            }
            
            
        }


        if (isDragging1) // 상단 링을 클릭한 상태라면
        {
           
            // 드래그한 정도 (마우스 거리 차) 구하기
            float dragDistance1 = (Input.mousePosition.x - initialMousePosition1.x);
            Debug.Log(dragDistance1);
            if (dragDistance1 > 1f || dragDistance1 < -1f)
            {
                // 누적 회전값은 적용하지 않음
                // Y축 갱신
                float newYRotation1 = (initialYRotation1 + dragDistance1) * rotationSpeed;
                
                RingObject1.transform.localRotation = Quaternion.Euler(0f, newYRotation1, 0f);

                if (rotationObject1 != null) // 충돌 방지
                {
                    float x = initialScale.x + dragDistance1 * 0.001f;
                    float y = initialScale.y + dragDistance1 * 0.001f;
                    float z = initialScale.z + dragDistance1 * 0.001f;
                    
                    rotationObject1.transform.localScale = new Vector3(x,y,z);

                    if (rotationObject1.transform.localScale.x <= 0.03f &&
                        rotationObject1.transform.localScale.y <= 0.03f &&
                        rotationObject1.transform.localScale.z <= 0.03f)
                    {
                        rotationObject1.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                    }
                    if (rotationObject1.transform.localScale.x >= 1f &&
                        rotationObject1.transform.localScale.y >= 1f &&
                        rotationObject1.transform.localScale.z >= 1f)
                    {
                        rotationObject1.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                }
            }
            
        }

        if (isDragging2) // 하단 링을 클릭한 상태라면
        {
          
            // 드래그한 정도 (마우스 거리 차) 구하기
            float dragDistance2 = (Input.mousePosition.x - initialMousePosition2.x);
            if (dragDistance2 > 1f || dragDistance2 < -1f)
            {
                // 누적 회전값은 적용하지 않음
                // Y축 갱신
                float newYRotation2 = (initialYRotation1 + dragDistance2) * rotationSpeed;
                RingObject2.transform.localRotation = Quaternion.Euler(0f, newYRotation2, 0f);

                if (rotationObject1 != null) // 충돌 방지
                {
                    // Y축 로컬 회전값 갱신

                    rotationObject1.transform.localRotation = initialRot * Quaternion.Euler(0f, newYRotation2, 0f);

                }
            }
            
        }
        

    }


}
