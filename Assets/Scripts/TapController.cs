using UnityEngine;
using System.Collections;
//using UnityEngine.Advertisements;

public class TapController : MonoBehaviour {

    private enum GameState {
        Initial,
        Active,
        GameOver
    }

	public GameObject artifact;
	public GameObject pillar;
	public GameObject[] pillarEffects;
	private bool isHolding = false;
    private GameState gameState = GameState.Initial;

    public Transform maxVector, minVector;

	private bool pillarRetracted = false;
	private float pillarRetractSpeed;
	public Transform pillarRetractStartMarker;
	public Transform pillarRetractEndMarker;
	private float pillarRetractStartTime;
	private float pillarRetractDuration = 2.0F;
    
    private Vector2 lastTouchPos = Vector2.zero;

    public bool adsActive = true;

    private int score = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (gameState != GameState.GameOver)
        {
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0)) {
                Ray touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector2 touchPos = new Vector2 (touchRay.origin.x, touchRay.origin.y);
                Collider2D collider;
                if ((collider = Physics2D.OverlapPoint(touchPos)) != null) {
                    Debug.Log ("touched" + collider + " " + collider.gameObject);
                    if (collider.gameObject == artifact) {
                        isHolding = true;
                        if (gameState != GameState.Active) {
                            gameState = GameState.Active;
                            pillarRetractStartTime = Time.time;
                            pillar.GetComponent<Collider2D>().enabled = false;
                        }
                        else
                        {
                            score++;
                        }
                    }
                }
            } else if (isHolding) {
                if (Input.GetMouseButton(0)) {
                    Ray touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Vector2 touchPos = new Vector2 (touchRay.origin.x, touchRay.origin.y);
                    if (touchPos.y > maxVector.position.y - 0.5F)
                    {
                        touchPos.y = maxVector.position.y - 0.5F;
                    }
                    if (touchPos.x > maxVector.position.x - 0.5F)
                    {
                        touchPos.x = maxVector.position.x - 0.5F;
                    }
                    if (touchPos.x < minVector.position.x + 0.5F)
                    {
                        touchPos.x = minVector.position.x + 0.5F;
                    }
                    artifact.transform.position = new Vector3 (touchPos.x, touchPos.y, artifact.transform.position.z);
                    artifact.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
                    lastTouchPos = touchPos;
                } else if (Input.GetMouseButtonUp(0)) {
                    Ray touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Vector2 touchPos = new Vector2 (touchRay.origin.x, touchRay.origin.y);
                    Vector2 touchDiff = touchPos - lastTouchPos;
                    Debug.Log(touchDiff);
                    artifact.GetComponent<Rigidbody2D>().AddForce(touchDiff * 30, ForceMode2D.Impulse);
                    isHolding = false;
                }
            }

            if (gameState == GameState.Active && !pillarRetracted) {
                AudioSource audio = pillar.GetComponent<AudioSource>();
                if (!audio.isPlaying) {
                    audio.Play();
                }
                foreach (GameObject effect in pillarEffects) {
                    effect.GetComponent<SpriteRenderer> ().enabled = true;
                }
                float lerpTime = (Time.time - pillarRetractStartTime) / pillarRetractDuration;
                pillar.transform.position = Vector3.Lerp(pillarRetractStartMarker.position, pillarRetractEndMarker.position, lerpTime);
                if (lerpTime >= 1) {
                    audio.Stop();
                    pillarRetracted = true;
                    foreach (GameObject effect in pillarEffects) {
                        effect.GetComponent<SpriteRenderer> ().enabled = false;
                    }
                }
            }
        }
	}

    public void GameOver() {
        gameState = GameState.GameOver;
    }

    void ResetGame() {
        gameState = GameState.Initial;
        score = 0;
        pillarRetracted = false;
        AudioSource audio = pillar.GetComponent<AudioSource>();
        audio.Stop();
        pillar.transform.position = pillarRetractStartMarker.position;
        pillar.GetComponent<Collider2D>().enabled = true;
        artifact.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        artifact.transform.rotation = Quaternion.identity;
        artifact.GetComponent<ArtifactController>().Reset();
        foreach (GameObject effect in pillarEffects) {
            effect.GetComponent<SpriteRenderer> ().enabled = false;
        }
        if (adsActive)
        {
            ShowAd();
        }
    }

    public void ShowAd()
    {
//        if (Advertisement.IsReady())
//        {
//            Advertisement.Show();
//        }
    }

    void OnGUI()
    {
        if (gameState == GameState.Active)
        {
            var labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.alignment = TextAnchor.UpperCenter;
            labelStyle.fontSize = 60;
            GUI.Label(new Rect (Screen.width / 2 - 150, 10, 200, 100), "Score:", labelStyle);
            GUI.Label(new Rect (Screen.width / 2 + 50, 10, 100, 100), "" + score, labelStyle);
        }
        else if (gameState == GameState.GameOver)
        {
            var labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.alignment = TextAnchor.UpperCenter;
            labelStyle.fontSize = 80;
            GUI.Label(new Rect (50, Screen.height / 2 - 200, Screen.width - 100, 100), "Game Over!", labelStyle);
            GUI.Label(new Rect (Screen.width / 2 - 200, Screen.height / 2 - 100, 250, 100), "Score:", labelStyle);
            GUI.Label(new Rect (Screen.width / 2 + 60, Screen.height / 2 - 100, 200, 100), "" + score, labelStyle);
            var buttonStyle = GUI.skin.GetStyle("Button");
            buttonStyle.alignment = TextAnchor.UpperCenter;
            buttonStyle.fontSize = 80;
            if (GUI.Button(new Rect (Screen.width / 2 - 200, Screen.height / 2 - 0, 400, 100), "Restart", buttonStyle))
            {
                ResetGame();
            }
        }
    }
}