using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileDownloader : MonoBehaviour
{
    // 远程文件的URL
    public string fileUrl = "https://i0.hdslb.com/bfs/archive/15d4d4effe4a9ad8bc80b34c0f02f523896b40f0.jpg";
    
    // 文件保存的本地路径
    private string localFilePath;

    void Start()
    {
        // 开始下载文件
        StartCoroutine(DownloadFile(fileUrl));
    }

    IEnumerator DownloadFile(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 开始下载
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Download failed: " + request.error);
            }
            else
            {
                // 获取下载的数据
                byte[] fileData = request.downloadHandler.data;

                // 将数据写入本地文件
                string localPath = Path.Combine(Application.persistentDataPath, "yourfile.png");
                File.WriteAllBytes(localPath, fileData);

                Debug.Log("File successfully downloaded and saved to: " + localPath);
            }
        }
    }
}
