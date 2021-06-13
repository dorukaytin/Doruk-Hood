﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float mySpeedX;
    private Rigidbody2D myBody;
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    private Vector3 defaultLocalScale;
    public bool onGround;
    private bool canDoubleJump;
    [SerializeField] GameObject arrow;
    [SerializeField] bool attacked;
    [SerializeField] float currentAttackTimer;
    [SerializeField] float defaultAttackTimer;
    private Animator myAnimator;
    public int arrowNumber;
    public Text arrowNumberText;
    [SerializeField] AudioClip dieMusic;
    [SerializeField] GameObject winPanel, losePanel;
    // Start is called before the first frame update
    void Start()
    {
        attacked = false;
        myAnimator = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody2D>();
        defaultLocalScale = transform.localScale;
        arrowNumberText.text = arrowNumber.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("Horizontal"));
        mySpeedX = Input.GetAxis("Horizontal");
        myAnimator.SetFloat("Speed", Mathf.Abs(mySpeedX));

        myBody.velocity = new Vector2(mySpeedX * speed, myBody.velocity.y);
        if (mySpeedX < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        #region playerin sağ ve sol hareket yönüne  göre yüzünün dönmesi
        else if (mySpeedX > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        #endregion
        #region playerin zıplamasının kontrol edilmesi.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("zıpla");
            if (onGround == true)
            {
                myBody.velocity = new Vector2(myBody.velocity.x, jumpPower);
                canDoubleJump = true;
                myAnimator.SetTrigger("jump");
            }
            else
            {
                if(canDoubleJump == true)
                {
                    myBody.velocity = new Vector2(myBody.velocity.x, jumpPower);
                    canDoubleJump = false;
                }
            }
            
        }
        #endregion
        #region playerin ok atmasının kontrolü
        if (Input.GetMouseButtonDown(0) && arrowNumber>0)
        {
            if (attacked==false)
            {
                attacked = true;
                myAnimator.SetTrigger("attack");
                Invoke("Fire", 0.5f); 
            }
            
            
            
        }
        #endregion
        if (attacked == true)
        {
            
            currentAttackTimer -= Time.deltaTime;
            
        }
        else
        {
            currentAttackTimer = defaultAttackTimer;
        }

        if (currentAttackTimer <= 0)
        {
            attacked = false;
        }
    }
    void Fire()
    {
        GameObject okumuz = Instantiate(arrow, transform.position, Quaternion.identity);
        okumuz.transform.parent = GameObject.Find("Arrows").transform;

        if (transform.localScale.x > 0)
        {
            okumuz.GetComponent<Rigidbody2D>().velocity = new Vector2(5f, 0f);
        }
        else
        {
            Vector3 okumuzScale = okumuz.transform.localScale;
            okumuz.transform.localScale = new Vector3(-okumuzScale.x, okumuzScale.y, okumuzScale.z);
            okumuz.GetComponent<Rigidbody2D>().velocity = new Vector2(-5f, 0f);
        }
        arrowNumber--;
        arrowNumberText.text = arrowNumber.ToString();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GetComponent<TimeCotroller>().enabled = false;
            die();
        }
        else if (collision.gameObject.CompareTag("Finis"))
        {
            /* winPanel.active = true;
             Time.timeScale = 0;*/
            GameObject.Destroy(collision.gameObject);
            StartCoroutine(Wait(true));
        }
    }
    public void die()
    {
        GameObject.Find("Sound Controller").GetComponent<AudioSource>().clip = null;
        GameObject.Find("Sound Controller").GetComponent<AudioSource>().PlayOneShot(dieMusic);
        myAnimator.SetFloat("Speed", 0);
        myAnimator.SetTrigger("die");

        myBody.constraints = RigidbodyConstraints2D.FreezeAll;

        enabled = false;
        //losePanel.SetActive(true);
        //Time.timeScale = 0;
        StartCoroutine(Wait(false));

        
    }
    IEnumerator Wait(bool win)
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0;

        if (win==true)
        {
            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }
       
    }
}
