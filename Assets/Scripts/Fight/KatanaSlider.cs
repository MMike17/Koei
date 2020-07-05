using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class KatanaSlider : MonoBehaviour, IPointerUpHandler
{
	[Header("Settings")]
	public float slideDuration;
	[Space]
	public float particlesLifetime;
	public float particlesUpForce, particlesSideForce, particleSpawnDelay, particleSpawnMaxOffset, initialParticleSize, endParticleSize;
	public Color particleColor;
	public Sprite[] particleSprites;

	[Header("Assign in Inspector")]
	public Image particlePrefab;
	public Transform spawnPoint;

	public Slider slider => GetComponent<Slider>();
	public bool done => slider.value == 1;

	Animator animator => GetComponent<Animator>();
	float step => (Mathf.Max(initialParticleSize, endParticleSize) - Mathf.Min(initialParticleSize, endParticleSize)) / particlesLifetime * Time.deltaTime;

	List<Particle> particles;
	float lastValue, particleTimer;
	bool startedSound;

	void OnDrawGizmos()
	{
		if(spawnPoint != null)
			Debug.DrawLine(spawnPoint.position - Vector3.right * particleSpawnMaxOffset, spawnPoint.position + Vector3.right * particleSpawnMaxOffset, Color.red);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if(slider.value > 0.1f)
			StartCoroutine(Move());
		else
			animator.Play("Fade");
	}

	void Awake()
	{
		slider.onValueChanged.AddListener(OnValueChange);

		particles = new List<Particle>();
	}

	void Update()
	{
		if(lastValue != slider.value)
		{
			particleTimer += Time.deltaTime;

			if(slider.value >= 1)
				AudioManager.PlaySound("KatanaEnd");
		}

		if(particleTimer >= particleSpawnDelay)
		{
			particleTimer = 0;

			SpawnParticles();
		}

		CheckParticle();

		lastValue = slider.value;
	}

	public void ForceSetValue(float value)
	{
		slider.onValueChanged.RemoveAllListeners();
		slider.value = value;
		slider.onValueChanged.AddListener(OnValueChange);
		startedSound = false;
	}

	// destroy old particles
	void CheckParticle()
	{
		List<Particle> toDelete = new List<Particle>();

		foreach (Particle particle in particles)
		{
			if(particle.timer >= particlesLifetime)
			{
				Destroy(particle.transform.gameObject);
				toDelete.Add(particle);
			}
			else
				particle.SetVelocityOrientation(step);
		}

		toDelete.ForEach(item => particles.Remove(item));
	}

	void SpawnParticles()
	{
		// set sprite
		Image spawned = Instantiate(particlePrefab, spawnPoint);
		spawned.sprite = particleSprites[Random.Range(0, particleSprites.Length - 1)];
		spawned.color = particleColor;

		// set sprite direction
		float side = Random.Range(-1f, 1f);

		if(side < 0)
			spawned.transform.localScale = new Vector3(initialParticleSize, -initialParticleSize, initialParticleSize);

		// set position
		spawned.transform.position = spawnPoint.position + Vector3.right * side * particleSpawnMaxOffset;

		// add motion
		Particle particle = new Particle(spawned.transform);
		float mass = particle.rigidbody2D.mass;
		particle.rigidbody2D.AddForce(Vector2.up * particlesUpForce * mass + Vector2.right * side * particlesSideForce * mass / 2, ForceMode2D.Impulse);

		particles.Add(particle);
	}

	void OnValueChange(float value)
	{
		animator.Play("Idle");

		if(!startedSound)
		{
			startedSound = true;
			AudioManager.PlaySound("DrawKatana");
		}
	}

	IEnumerator Move()
	{
		while (slider.value != 1)
		{
			slider.interactable = false;
			slider.value = Mathf.MoveTowards(slider.value, 1, 1 / slideDuration * Time.deltaTime);

			yield return null;
		}

		slider.interactable = true;
		yield break;
	}

	class Particle
	{
		public Transform transform;
		public float timer;

		public Rigidbody2D rigidbody2D => transform.GetComponent<Rigidbody2D>();

		public Particle(Transform particle)
		{
			transform = particle;
		}

		public void Init()
		{
			timer = 0;
		}

		public void SetVelocityOrientation(float scaleStep)
		{
			timer += Time.deltaTime;

			float sign = Mathf.Sign(transform.localScale.y);
			float newScale = transform.localScale.x - scaleStep;
			transform.localScale = new Vector3(newScale, sign * newScale, newScale);

			if(transform == null)
				return;

			if(Vector3.Angle(transform.position + (Vector3) rigidbody2D.velocity, transform.right) > 0.1f)
				transform.Rotate(0, 0, -Vector3.SignedAngle(rigidbody2D.velocity, transform.right, Vector3.forward));
		}
	}
}