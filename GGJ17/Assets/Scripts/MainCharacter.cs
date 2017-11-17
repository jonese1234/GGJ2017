using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MainCharacter : MonoBehaviour {

    public AudioClip jump;
    public AudioClip death;
    public AudioClip pickup;
    public Text timeTxt;

    public int moveSpeed;
    public int jumpHeight;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    private bool grounded;

    public bool doubleJumped;

    private string currentColour = "";

    private bool redEnabled = true;
    private bool blueEnabled = false;
    private bool greenEnabled = false;
    private bool doubleJumpEnabled = false;

    private bool dead = false;

    private int deathCount = 0;
    public Text deathLabel;

    private bool onPlatform = false;
    private GameObject currentPlatform;

    public Text helpLabel;
    public Image helpImg;
    private Dictionary<char,float> checkpoint = new Dictionary<char,float>();

    public LayerMask whatIsPlatform;

    Animator animator;

    private const int MAX_PHOTON_NUMBER = 37;
    private int collectedPhotons = 0;
    public Text photonLabel;
    private AudioSource audio;

    string HELP_BOX_ONE = "Life is hard sometimes, well now you're a light wave... One advantage is you can move through some blocks.",
        HELP_BOX_TWO = "Some say light should be unaffected by spikes.... They are idiots.",
        HELP_BOX_THREE = "Whoever built this place must have known you would end up here.",
        HELP_BOX_FOUR = "Pick that green thing up, I don't think it's radioactive. Probably just the light. (Change with the number keys)",
        HELP_BOX_FIVE = "Hey looks like you can defy gravity.",
        HELP_BOX_SIX = "It's blue da ba dee da ba daa",
        HELP_BOX_SEVEN = "KYS";

    void setHelpBoxText(string name) {
        helpImg.GetComponent<Image>().enabled = true;
        if (name.Contains("0")) {
            helpLabel.text = HELP_BOX_ONE;
        }else if (name.Contains("1")) {
            helpLabel.text = HELP_BOX_TWO;
        } else if (name.Contains("2")) {
            helpLabel.text = HELP_BOX_THREE;
        } else if (name.Contains("3")) {
            helpLabel.text = HELP_BOX_FOUR;
        } else if (name.Contains("4")) {
            helpLabel.text = HELP_BOX_FIVE;
        }else if (name.Contains("5")){
            helpLabel.text = HELP_BOX_SIX;
        }else if (name.Contains("6")){
            helpLabel.text = HELP_BOX_SEVEN;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.name.ToString().ToLower().Contains("checkpoint") || collider.gameObject.name.ToString().ToLower().Contains("startpoint"))
        {
            Dictionary<char, float> coords = new Dictionary<char, float>();
            coords.Add('x', collider.gameObject.transform.position.x);
            coords.Add('y', collider.gameObject.transform.position.y + GetComponent<Renderer>().bounds.size.y);

            checkpoint = coords;
        }
        else if (collider.gameObject.name.ToString().ToLower().Contains("helpbox"))
        {
            setHelpBoxText(collider.gameObject.name.ToString().ToLower());
        }
        else if (collider.gameObject.name.ToString().ToLower().Contains("greenpickup"))
        {
            //enable green
            playPickup();
            Destroy(collider.gameObject);
            greenEnabled = true;
        }
        else if (collider.gameObject.name.ToString().ToLower().Contains("bluepickup"))
        {
            //enable blue
            playPickup();
            Destroy(collider.gameObject);
            blueEnabled = true;

        }
        else if (collider.gameObject.name.ToString().ToLower().Contains("jumppickup"))
        {
            //enable double jump
            playPickup();
            Destroy(collider.gameObject);
            doubleJumpEnabled = true;
        }
        else if (collider.gameObject.name.ToString().ToLower().Contains("photon"))
        {
            //collect photon
            playPickup();
            Destroy(collider.gameObject);
            collectedPhotons++;
            photonLabel.text = "Photons: " + collectedPhotons + "/37";
            if(collectedPhotons == MAX_PHOTON_NUMBER)
            {
                endGame();
            }
        }

    }

    void endGame()
    {
        string time = timeTxt.text;
        PlayerPrefs.SetString("time", time);
        PlayerPrefs.SetInt("deaths", deathCount);
        PlayerPrefs.Save();
        SceneManager.LoadScene(2);
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.name.ToString().ToLower().Contains("helpbox")) {
            helpLabel.text = "";
            helpImg.GetComponent<Image>().enabled = false;
        }
    }
    void FixedUpdate() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,whatIsGround);

        var objs = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius,whatIsPlatform);
       
        if (objs.Length > 0) {
            GameObject obj = objs[0].gameObject;
            if (obj.transform.parent.name.ToLower().Contains("platform")) {
                onPlatform = true;
                currentPlatform = obj;
                return;
            }
        }
        onPlatform = false;

       
    }
    void changeColour(string colour) {
        //turn previous colour blocks back on
        switch (currentColour) {
            case "red":
                makeRedBlocksVisible();
                break;
            case "blue":
                makeBlueBlocksVisible();
                break;
            case "green":
                makeGreenBlocksVisible();
                break;
        }
        //turn next colour blocks off
        switch (colour.ToLower()) {
            case "red":
                makeRedBlocksTransparent();
                GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Art/RedMan_0") as RuntimeAnimatorController;
                break;
            case "blue":
                makeBlueBlocksTransparent();
                GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Art/BlueMan_0") as RuntimeAnimatorController;
                break;
            case "green":
                makeGreenBlocksTransparent();
               GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Art/GreenMan_0") as RuntimeAnimatorController;
                break;

        }
        //change colour flag
        currentColour = colour.ToLower();
    }

    // Use this for initialization

    void Start () {
       GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = GetComponent<Animator>();
        changeColour("red");
        audio = GetComponent<AudioSource>();
       

        //turnOffPlatforms();
    }
	
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.transform.parent.name.ToString().ToLower().Contains("spikes")) {
            killPlayer();
        }
        
       

    }

    void playJump()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("sfxVolume",1.0f);
        audio.PlayOneShot(jump);
    }
    void playDeath()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("sfxVolume",1.0f);
        audio.PlayOneShot(death);
    }
    void playPickup()
    {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("sfxVolume",1.0f);
        audio.PlayOneShot(pickup);
    }


    void killPlayer() {
        //oh no you died
        playDeath();
        dead = true;
        deathCount++;
        deathLabel.text = "Deaths: " + deathCount.ToString();
        GetComponent<Rigidbody2D>().position = new Vector2(checkpoint['x'], checkpoint['y']);
        dead = false;

    }

    // Update is called once per frame
    void Update () {
        var currentVelocity = GetComponent<Rigidbody2D>().velocity;

        if (grounded || onPlatform) {
            doubleJumped = false;
            
        }

        if (!dead) {
            if (Input.GetKey(KeyCode.RightArrow)) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, currentVelocity.y);

            }

            if (Input.GetKey(KeyCode.LeftArrow)) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, currentVelocity.y);
            }

            if (Input.GetKeyDown(KeyCode.Space) && (grounded || onPlatform)) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(currentVelocity.x, jumpHeight);
                playJump();
            }

            if (Input.GetKeyDown(KeyCode.Space) && (!grounded || !onPlatform) && !doubleJumped && doubleJumpEnabled) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(currentVelocity.x, jumpHeight);
                doubleJumped = true;
                playJump();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && redEnabled) {
                changeColour("red");
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && greenEnabled) {
                changeColour("green");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && blueEnabled) {
                changeColour("blue");
            }
            if (Input.GetKeyDown(KeyCode.K)) {
                killPlayer();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && onPlatform) {
                StartCoroutine("togglePlatforms");    
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }

        }
        
        

    }
    private bool platformsOn = true;
    DateTime start;
    DateTime End;
    IEnumerator togglePlatforms() {
       
        for (int i = 0; i < 2; i++) {
           
            var platforms = GameObject.FindGameObjectsWithTag("platform");
            if (platformsOn) {
                start = DateTime.Now;
                
                foreach (GameObject platform in platforms) {
                    platform.gameObject.transform.GetComponent<PolygonCollider2D>().enabled = false;

                }
                platformsOn = false;
            } else {
                
                foreach (GameObject platform in platforms) {
                    platform.gameObject.transform.GetComponent<PolygonCollider2D>().enabled = true;

                }
                platformsOn = true;
                End = DateTime.Now;
                Debug.Log((End - start).Milliseconds - 400);

            }

            yield return new WaitForSeconds(.4f);
        }
        
        

    }

    void makeBlueBlocksTransparent() {
        List<GameObject> transparentObjects = new List<GameObject>();
        //find blocks to add to be transparent
        var blocks = GameObject.FindGameObjectsWithTag("blueBlock");
        //add blocks to list
        foreach(GameObject block in blocks) {         
                transparentObjects.Add(block);            
        }


        //make list items transparent and non collidable
        foreach(GameObject obj in transparentObjects) {
            var colour = obj.GetComponent<Renderer>().material.color;
            //change colour
            var newColour = new Color(colour.r ,colour.g,colour.b,0.3f);
            obj.GetComponent<Renderer>().material.color = newColour;

            //turn off the collider
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = false;      
        }
    }

    void makeRedBlocksTransparent() {
        List<GameObject> transparentObjects = new List<GameObject>();
        //find blocks to add to be transparent
        var blocks = GameObject.FindGameObjectsWithTag("redBlock");
        //add blocks to list
        foreach (GameObject block in blocks) {
            transparentObjects.Add(block);
        }


        //make list items transparent and non collidable
        foreach (GameObject obj in transparentObjects) {
            var colour = obj.GetComponent<Renderer>().material.color;
            //change colour
            var newColour = new Color(colour.r, colour.g, colour.b, 0.3f);
            obj.GetComponent<Renderer>().material.color = newColour;

            //turn off the collider
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        }
    }

    void makeGreenBlocksTransparent() {
        List<GameObject> transparentObjects = new List<GameObject>();
        //find blocks to add to be transparent
        var blocks = GameObject.FindGameObjectsWithTag("greenBlock");
        //add blocks to list
        foreach (GameObject block in blocks) {
            transparentObjects.Add(block);
        }


        //make list items transparent and non collidable
        foreach (GameObject obj in transparentObjects) {
            var colour = obj.GetComponent<Renderer>().material.color;
            //change colour
            var newColour = new Color(colour.r, colour.g, colour.b, 0.3f);
            obj.GetComponent<Renderer>().material.color = newColour;

            //turn off the collider
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        }
    }

    void makeBlueBlocksVisible() {
        List<GameObject> visibleBlocks = new List<GameObject>();
        //find blocks to add to be visible
        var blocks = GameObject.FindGameObjectsWithTag("blueBlock");
        //add blocks to list
        foreach (GameObject block in blocks) {
            visibleBlocks.Add(block);
        }


        //make list items transparent and non collidable
        foreach (GameObject obj in visibleBlocks) {
            var colour = obj.GetComponent<Renderer>().material.color;
            //change colour
            var newColour = new Color(colour.r, colour.g, colour.b, 1.0f);
            obj.GetComponent<Renderer>().material.color = newColour;

            //turn off the collider
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }


    void makeRedBlocksVisible() {
        List<GameObject> visibleBlocks = new List<GameObject>();
        //find blocks to add to be visible
        var blocks = GameObject.FindGameObjectsWithTag("redBlock");
        //add blocks to list
        foreach (GameObject block in blocks) {
            visibleBlocks.Add(block);
        }


        //make list items transparent and non collidable
        foreach (GameObject obj in visibleBlocks) {
            var colour = obj.GetComponent<Renderer>().material.color;
            //change colour
            var newColour = new Color(colour.r, colour.g, colour.b, 1.0f);
            obj.GetComponent<Renderer>().material.color = newColour;

            //turn off the collider
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }

    void makeGreenBlocksVisible() {
        List<GameObject> visibleBlocks = new List<GameObject>();
        //find blocks to add to be visible
        var blocks = GameObject.FindGameObjectsWithTag("greenBlock");
        //add blocks to list
        foreach (GameObject block in blocks) {
            visibleBlocks.Add(block);
        }


        //make list items transparent and non collidable
        foreach (GameObject obj in visibleBlocks) {
            var colour = obj.GetComponent<Renderer>().material.color;
            //change colour
            var newColour = new Color(colour.r, colour.g, colour.b, 1.0f);
            obj.GetComponent<Renderer>().material.color = newColour;

            //turn off the collider
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }

    void turnOffPlatforms() {   
        var platforms = GameObject.FindGameObjectsWithTag("platform");
        foreach (GameObject obj in platforms) {
            obj.transform.parent.FindChild("Collision").gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        }
    }


}
