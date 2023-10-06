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
    public GameObject RingObject1; // Ray �浹üũ�� �޽� �ݶ��̴�
    public GameObject rotationObject1; // ȸ������ ��ġ��ų ������Ʈ
    public bool isDragging1 = false; // �巡�� ����
    private float initialYRotation1; // �ʱ� ȸ����
    private Vector3 initialMousePosition1; // �ʱ� ���콺 ��ġ
    private float accumulatedRotation1 = 0f; // ���� ȸ����

    [Header("Bottom Ring")]
    public GameObject RingObject2; // Ray �浹üũ�� �޽� �ݶ��̴�
    //public GameObject rotationObject2; // ȸ������ ��ġ��ų ������Ʈ    
    public bool isDragging2 = false; // �巡�� ����
    private float initialYRotation2; // �ʱ� ȸ����
    private Vector3 initialMousePosition2; // �ʱ� ���콺 ��ġ
    private float accumulatedRotation2 = 0f; // ���� ȸ����

    [Header("Core")]
    public bool isDragging3 = false;
    private float currentTime;
    public float moveStationaryTime = 0.7f;
    private bool isMoved = false;
    public GameObject oopartObj;
    public ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hitInfos = new List<ARRaycastHit>(); //�浹 ��ü ������ hits
    public TextMeshProUGUI tm;
    public float rotationSpeed = 0.01f; // ȸ����ų ����(�ӵ�)
    void Start()
    {
        // ���� ARCamera�� ����ִ� ��쿡�� MainCam �ֱ�
        if (arCamera == null) { arCamera = Camera.main; }
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ��
        {
            
            //2. ���콺 Ŭ���� ��ġ���� Ray�� �߻�
            Vector3 mousePos = Input.mousePosition;
            //1. Ray : Mouse Ŭ���� ��ġ
            Ray ray = arCamera.ScreenPointToRay(mousePos); ; // ���콺 �� ��ġ���� ���� ���
            //2. RaycastHit : �浹�� ��ü ��� �׸�
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
                // ���̰� �浹�� �� ����

                if (rotationObject1 != null)
                {
                    if (hit.collider.CompareTag("Ring1")) // ��� ���� ������
                    {
                        initialScale = rotationObject1.transform.localScale;

                        // ȸ���� ����
                        isDragging1 = true;
                        initialYRotation1 = RingObject1.transform.localEulerAngles.y;
                        initialMousePosition1 = Input.mousePosition;
                        accumulatedRotation1 = 0f; // ���� Ŭ���� ������ ���� ȸ���� �ʱ�ȭ

                    }

                    if (hit.collider.CompareTag("Ring2")) // �ϴ� ���� ������
                    {
                        initialRot = rotationObject1.transform.localRotation;
                        //// ȸ���� ����
                        isDragging2 = true;
                        initialYRotation2 = RingObject2.transform.localEulerAngles.y;
                        initialMousePosition2 = Input.mousePosition;
                        accumulatedRotation2 = 0f; // ���� Ŭ���� ������ ���� ȸ���� �ʱ�ȭ

                    }

                    if (hit.collider.CompareTag("Core")) // �ھ ������
                    {

                        //isDragging3 = true;
                        
                    }
                } else
                {
                    Debug.Log("������Ʈ�� ����ֽ��ϴ�.");
                }
               

               
            }

        }
        else if (Input.GetMouseButtonUp(0)) // ���콺 ����
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


        if (isDragging1) // ��� ���� Ŭ���� ���¶��
        {
           
            // �巡���� ���� (���콺 �Ÿ� ��) ���ϱ�
            float dragDistance1 = (Input.mousePosition.x - initialMousePosition1.x);
            Debug.Log(dragDistance1);
            if (dragDistance1 > 1f || dragDistance1 < -1f)
            {
                // ���� ȸ������ �������� ����
                // Y�� ����
                float newYRotation1 = (initialYRotation1 + dragDistance1) * rotationSpeed;
                
                RingObject1.transform.localRotation = Quaternion.Euler(0f, newYRotation1, 0f);

                if (rotationObject1 != null) // �浹 ����
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

        if (isDragging2) // �ϴ� ���� Ŭ���� ���¶��
        {
          
            // �巡���� ���� (���콺 �Ÿ� ��) ���ϱ�
            float dragDistance2 = (Input.mousePosition.x - initialMousePosition2.x);
            if (dragDistance2 > 1f || dragDistance2 < -1f)
            {
                // ���� ȸ������ �������� ����
                // Y�� ����
                float newYRotation2 = (initialYRotation1 + dragDistance2) * rotationSpeed;
                RingObject2.transform.localRotation = Quaternion.Euler(0f, newYRotation2, 0f);

                if (rotationObject1 != null) // �浹 ����
                {
                    // Y�� ���� ȸ���� ����

                    rotationObject1.transform.localRotation = initialRot * Quaternion.Euler(0f, newYRotation2, 0f);

                }
            }
            
        }
        

    }


}
