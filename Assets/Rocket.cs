using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;
    Rigidbody rigidbody;
    AudioSource audioSource;

    public State state = State.Alive;

    public enum State
    {
        Alive, Dying, Transcending
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state != State.Alive)
            return;

        ResponToThrustInput();
        ResponToRotateInput();
    }

    void OnCollisionEnter(Collision other)
    {
        if (state != State.Alive)
            return;

        switch (other.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                this.state = State.Transcending;
                audioSource.PlayOneShot(success);
                successParticles.Play();
                Invoke("LoadNextScene", 1f);
                break;
            default:
                this.state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                deathParticles.Play();
                Invoke("LoadFirstScene", 1f);
                break;
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void ResponToRotateInput()
    {
        rigidbody.freezeRotation = true;

        float rotateThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotateThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotateThisFrame);
        }
        rigidbody.freezeRotation = false;
    }

    private void ResponToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(mainEngine);

        if (!mainEngineParticles.isPlaying)
            mainEngineParticles.Play();
    }
}
