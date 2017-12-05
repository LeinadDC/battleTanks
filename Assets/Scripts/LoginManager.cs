using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;
using LitJson;

public class LoginManager : MonoBehaviour {

    public InputField password;
    public InputField email;
    public Button loginButton;
    public Button registerButton;
    public static string userToken;

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
            Debug.Log(request.downloadHandler.text);
            JsonData jsonServer = JsonMapper.ToObject(request.downloadHandler.text);
            string token = jsonServer["access_token"].ToString();
            userToken = token;
            SceneManager.LoadScene("MainGame");
        
        }
    }

    public string getUserToken()
    {
        return userToken;
    }
    

    IEnumerator Test(string bodyJsonString)
    {
        WWWForm form = new WWWForm();
        var headers  =new Hashtable(form.headers);
        string url = "www.myurl.com";
        // Add a custom header to the request.
        // In this case a basic authentication to access a password protected resource.
        headers["Authorization"] = "Bearer " + System.Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes("username:password"));
        headers.Add("Content-Type", "application/json");

        // Post a request to an URL with our custom headers
        WWW www = new WWW(url, Encoding.ASCII.GetBytes(bodyJsonString), headers);
        yield return www;
        //.. process results from WWW request here...
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
