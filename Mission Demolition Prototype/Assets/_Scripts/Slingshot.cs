using System.Collections;
using UnityEngine;

public class Slingshot : MonoBehaviour {
	static private Slingshot S;

	[Header("Set in Inspector")]
	public float velocityMult = 10f;
	public GameObject prefabProjectile;

	[Header("Set Dynamically")]
	public bool aimingMode;
    public bool canShoot;
	public GameObject launchPoint;
	public GameObject projectile;
	public Vector3 launchPos;

    private LineRenderer slingLine;
    private Rigidbody projectileRigidbody;
    
	static public Vector3 LAUNCH_POS {
		get {
			if (S == null) return Vector3.zero;
			return S.launchPos;
		}
	}

	void Awake() {
		S = this;
        canShoot = true;
		Transform launchPointTrans = transform.Find("LaunchPoint");
		launchPoint = launchPointTrans.gameObject;
		launchPoint.SetActive(false);
		launchPos = launchPointTrans.position;

        slingLine = GetComponent<LineRenderer>();
        slingLine.enabled = false;
	}

	void OnMouseEnter()	{
		//print("Slingshot:OnMouseEnter()");
		launchPoint.SetActive(true);
	}

	void OnMouseExit()	{
		//print("Slingshot:OnMouseExit()");
		launchPoint.SetActive(false);
	}

	void OnMouseDown()	{
        if (canShoot)
        {
            aimingMode = true;
            projectile = Instantiate(prefabProjectile) as GameObject;
            projectile.transform.position = launchPos;
            projectile.GetComponent<Rigidbody>().isKinematic = true;

            projectileRigidbody = projectile.GetComponent<Rigidbody>();
            projectileRigidbody.isKinematic = true;
        }
	}

	void Update() {
        if (!aimingMode) return;

		Vector3 mousePos2D = Input.mousePosition;
		mousePos2D.z = -Camera.main.transform.position.z;
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

		Vector3 mouseDelta = mousePos3D - launchPos;

		float maxMagnitude = this.GetComponent<SphereCollider>().radius;
		if (mouseDelta.magnitude > maxMagnitude)
		{
			mouseDelta.Normalize();
			mouseDelta *= maxMagnitude;
		}

		Vector3 projPos = launchPos + mouseDelta;
		projectile.transform.position = projPos;

        // Display slingLine from slingshot to cursor
        slingLine.SetPosition(0, launchPos);
        slingLine.SetPosition(1, projPos);
        slingLine.enabled = true;

            if (canShoot && Input.GetMouseButtonUp(0))
            {
                aimingMode = false;
                projectileRigidbody.isKinematic = false;
                projectileRigidbody.velocity = -mouseDelta * velocityMult;
                FollowCam.POI = projectile;
                projectile = null;
                MissionDemolition.ShotFired();
                canShoot = false;
                Invoke("AllowShooting", 3);
                ProjectileLine.S.poi = projectile;
                slingLine.enabled = false;
            }
    }

    private void AllowShooting()
    {
        canShoot = true;
    }
}
