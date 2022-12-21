using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float score = 0;
    public float totalCollectableItem;
    public FixedJoystick fixedJoystick;
    public Rigidbody rb;
    public TextMeshProUGUI score_text;

    public GameObject winPanel;
    public GameObject lostPanel;

    GameObject selectedObj;
    Renderer playerRenderer;
    Material defaultMaterial;

    float maxScore;

    public void Start()
    {
        playerRenderer = transform.GetChild(1).GetComponent<Renderer>();
        defaultMaterial = playerRenderer.material;
        totalCollectableItem = GameObject.FindGameObjectsWithTag("CollectableRed").Length + GameObject.FindGameObjectsWithTag("CollectableGreen").Length;
        maxScore = GameObject.FindGameObjectsWithTag("CollectableRed").Length * 5 + GameObject.FindGameObjectsWithTag("CollectableGreen").Length * 10;
    }

    public void FixedUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            JoystickController();
        }
    }

    private void JoystickController()
    {
        float horizontal = fixedJoystick.Horizontal;
        float vertical = fixedJoystick.Vertical;

        Vector3 position = new Vector3(horizontal * speed* Time.deltaTime, 0, vertical * speed * Time.deltaTime);
        transform.position += position;

        Vector3 rotation = Vector3.forward * vertical + Vector3.right * horizontal;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rotation), turnSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Desk"))
        {
            if (collision.transform.childCount > 0 && selectedObj == null)
            {
                int random = Random.Range(0, collision.transform.childCount);
                selectedObj = collision.transform.GetChild(random).gameObject;
                playerRenderer.material = selectedObj.GetComponent<Renderer>().material;
                collision.transform.GetChild(random).transform.parent = transform;
                selectedObj.transform.position = new Vector3(transform.position.x, 1.35f, transform.position.z);
            }
        }
        if (collision.gameObject.CompareTag("CubeGreen")) {
            if (selectedObj)
            {
                selectedObj.transform.parent = collision.transform;
                playerRenderer.material = defaultMaterial;
                if (selectedObj.tag == "CollectableGreen")
                {
                    IncreaseScore("green");
                }
                else
                {
                    DecreaseScore();
                }
                AfterStake();
            }
        }
        if (collision.gameObject.CompareTag("CubeRed"))
        {
            if (selectedObj)
            {
                selectedObj.transform.parent = collision.transform;
                playerRenderer.material = defaultMaterial;
                if (selectedObj.tag == "CollectableRed")
                {
                    IncreaseScore("red");
                }
                else
                {
                    DecreaseScore();
                }
                AfterStake();
            }
        }
        
    }

    private void AfterStake()
    {
        selectedObj = null;
        totalCollectableItem = totalCollectableItem - 1;
        if(totalCollectableItem == 0)
        {
            CheckScoreAndShow();
        }
    }

    private void DecreaseScore()
    {
        score = score - 5;
        UpdateScore();
    }

    private void IncreaseScore(string type)
    {
        if (type == "green") score = score + 10;
        if (type == "red") score = score + 5;
        UpdateScore();
    }

    private void UpdateScore()
    {
        score_text.text = "Score: " + score;
        CheckScoreAndShow();
    }

    private void CheckScoreAndShow()
    {
        if(score == maxScore)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0;
        }
        if(score < 0)
        {
            lostPanel.SetActive(true);
            Time.timeScale = 0;
        }
        if(totalCollectableItem == 0 && score < maxScore)
        {
            lostPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

}
