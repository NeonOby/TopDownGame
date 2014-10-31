using UnityEngine;
using System.Collections;

public class Healthbar : MonoBehaviour 
{
	private Transform mainCam;

	public Transform bar;

    public float scale = 0.1f;

	private float currentProcent = 1.0f;
	private float wantedProcent = 1.0f;

	public float speed = 2.0f;

	public void UpdateHealth(float procent){
		wantedProcent = procent;
	}

	void Start(){
		mainCam = Camera.main.transform;
	}

    public Texture barTex;
    public Texture overlayTex;
    public Texture bgTex;

    private Vector3 Pos
    {
        get
        {
            return transform.position;
        }
    }

    void OnGUI()
    {
        if (barTex == null || overlayTex == null)
            return;

        Vector3 pos = Camera.main.WorldToScreenPoint(Pos);
        GUI.DrawTexture(new Rect(pos.x - (bgTex.width / 2) * scale, Screen.height - pos.y, bgTex.width * scale, bgTex.height * scale), bgTex);
        GUI.DrawTexture(new Rect(pos.x - (barTex.width / 2) * scale, Screen.height - pos.y, barTex.width * scale * currentProcent, barTex.height * scale), barTex);
        GUI.DrawTexture(new Rect(pos.x - (overlayTex.width / 2) * scale, Screen.height - pos.y, overlayTex.width * scale, overlayTex.height * scale), overlayTex);
    }

	void Update(){
		currentProcent = Mathf.Lerp (currentProcent, wantedProcent, Time.deltaTime * speed);
		//bar.localScale = new Vector3(currentProcent * maxWidth, 1,1);
		//bar.localPosition = new Vector3((1.0f-currentProcent) * maxWidth/2.0f, 0,0);

		//transform.LookAt(mainCam);
	}

}
