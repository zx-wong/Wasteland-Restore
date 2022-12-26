using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeGunCollider : MonoBehaviour
{
	[SerializeField] private int damage = 50;

	[SerializeField] private float explosionForce = 4;
	[SerializeField] private float radius = 10f;
	[SerializeField] private float timeWait = 2f;

	[SerializeField] private GameObject explosion;

	[SerializeField] private bool collisionDetect = false;

	void ExecuteExplosion ()
	{
		float multiplier = 1f;
		PlayPartSysExplosion ();

		float r = radius * multiplier;
		var cols = Physics.OverlapSphere (transform.position, r);
		var rigidbodies = new List<Rigidbody> ();

		foreach (var col in cols) 
		{
			if (col.attachedRigidbody != null && !rigidbodies.Contains (col.attachedRigidbody)) 
			{
				rigidbodies.Add (col.attachedRigidbody);
			}


			if (col.gameObject.tag == "Enemy")
			{
				if (col.gameObject.name == "Mutant" || col.gameObject.name == "Mutant(Clone)")
				{
					col.transform.gameObject.GetComponent<MutantController>().HandleHealth(damage);
					col.transform.gameObject.GetComponent<MutantController>().getAttacked = true;
				}

				if (col.gameObject.name == "Creepy" || col.gameObject.name == "Creepy(Clone)")
				{
					col.transform.gameObject.GetComponent<CreepyController>().HandleHealth(damage);
					col.transform.gameObject.GetComponent<CreepyController>().getAttacked = true;
				}

				if (col.gameObject.name == "Skull" || col.gameObject.name == "Skull(Clone)")
				{
					col.transform.gameObject.GetComponent<SkullController>().HandleHealth(damage);
					col.transform.gameObject.GetComponent<SkullController>().getAttacked = true;
				}

				if (col.gameObject.name == "Spider" || col.gameObject.name == "Spider(Clone)")
				{
					col.transform.gameObject.GetComponent<SpiderController>().HandleHealth(damage);
					col.transform.gameObject.GetComponent<SpiderController>().getAttacked = true;
				}

				if (col.gameObject.name == "Slug" || col.gameObject.name == "Slug(Clone)")
				{
					col.transform.gameObject.GetComponent<SlugController>().HandleHealth(damage);
					col.transform.gameObject.GetComponent<SlugController>().getAttacked = true;
				}

				if (col.gameObject.name == "Boss" || col.gameObject.name == "Boss(Clone)")
				{
					col.transform.gameObject.GetComponent<GargoyleController>().HandleHealth(damage);
					//hitInfo.transform.gameObject.GetComponent<GargoyleController>().getAttacked = true;
				}
			}
		}
		
		foreach (var rb in rigidbodies) 
		{
			rb.AddExplosionForce (explosionForce * multiplier, transform.position, r, 1 * multiplier, ForceMode.Impulse);
		}
	}

	void PlayPartSysExplosion ()
	{
		var newExplosion = Instantiate (explosion);
		newExplosion.transform.position = this.transform.position;
		UnityStandardAssets.Effects.ParticleSystemMultiplier mpexplosion = newExplosion.GetComponent<UnityStandardAssets.Effects.ParticleSystemMultiplier> ();
		mpexplosion.ExecPartSystem ();
	}

	void OnCollisionEnter (Collision collision)
	{
		ExecuteExplosion ();
		Destroy (this.gameObject);
	}

}


