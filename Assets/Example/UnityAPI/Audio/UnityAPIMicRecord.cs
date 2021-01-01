using UnityEngine;
using UnityEngine.Audio;

public class UnityAPIMicRecord : MonoBehaviour
{
    public AudioSource audioSource = null;
    public AudioClip audioClip = null;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Microphone found");
        }
        else
        {
            Debug.Log("Microphone not found");
        }
    }
    private void Update()
    {
        //foreach(var device in Microphone.devices)
        //{
        //    Debug.Log($"{device}");
        //}

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"开始录制");
            //
            audioClip = Microphone.Start(Microphone.devices[2], true, 10, 44100);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"结束录制");
            Microphone.End(Microphone.devices[2]);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
