using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour {

    public InputField password;
    public InputField email;
    public Button loginButton;
    public Button registerButton;

    RegisterForm form;
    void Start()
    {
        password.onEndEdit.AddListener(SubmitPass);
        email.onEndEdit.AddListener(SubmitEmail);
        loginButton.onClick.AddListener(loginEvent);
        registerButton.onClick.AddListener(registerEvent);
        form = new RegisterForm();

    }

    private void loginEvent()
    {
        string jsonForm = JsonUtility.ToJson(form);
        login(jsonForm);
        Debug.Log(jsonForm);
    }

    private void registerEvent()
    {
        SceneManager.LoadScene("Register");
    }

    void login(string existingUser)
    {
        string requestUrl = "http://192.168.98.131:5000/login";
        StartCoroutine(Post(requestUrl, existingUser));
    }

    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            Debug.Log("Status Code: " + request.responseCode);
        }
        else
        {
            Debug.Log("Status Code: " + request.responseCode);
            SceneManager.LoadScene("MainGame");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void SubmitPass(string pass)
    {
        Debug.Log(pass);
        Debug.Log(password.text);
        form.password = pass;
    }

    private void SubmitEmail(string mail)
    {
        Debug.Log(mail);
        form.email = mail;
    }
}
