using UnityEngine;
using System.Collections;


public class CfgGlowCamera : MonoBehaviour
{
    public Camera m_cloneSource;
    public bool m_backupColor = false;
    public bool m_manualClearDepth = false;
    // Use this for initialization
    void Start()
    {

    }
    void Awake()
    {
    }
    public RenderBuffer m_mainDepthBuffer;
    void OnPreRender()
    {
        if (m_manualClearDepth)
        {
            //Graphics.bu
            if (GetComponent<Camera>().targetTexture != null)
            {
                //RenderBuffer[] rb = new RenderBuffer[] { camera.targetTexture.colorBuffer };
                //camera.SetTargetBuffers(rb, m_mainDepthBuffer);
                //camera.SetTargetBuffers(rb, camera.targetTexture.depthBuffer);
            }
            //RenderTexture.active = m_target2;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            //RenderTexture.active = m_target1;
            //Graphics.SetRenderTarget(m_buffers, m_target1.depthBuffer);
        }
    }
    public static void Follow(Camera src, Camera dst)
    {
        int cullingMask = dst.cullingMask;
        float backupDepth = dst.depth;
        RenderTexture backupTexture = dst.targetTexture;
        Color backupColor = dst.backgroundColor;
        CameraClearFlags clearFlags = dst.clearFlags;
        dst.CopyFrom(src);
        dst.cullingMask = cullingMask;
        dst.clearFlags = clearFlags;
        dst.targetTexture = backupTexture;
        dst.depth = backupDepth;
        dst.backgroundColor = backupColor;
    }
    // Update is called once per frame
    void Update()
    {
        if (m_cloneSource != null)
        {
            int cullingMask = GetComponent<Camera>().cullingMask;
            float backupDepth = GetComponent<Camera>().depth;
            RenderTexture backupTexture = GetComponent<Camera>().targetTexture;
            Color backupColor = GetComponent<Camera>().backgroundColor;
            CameraClearFlags clearFlags = GetComponent<Camera>().clearFlags;
            GetComponent<Camera>().CopyFrom(m_cloneSource);
            GetComponent<Camera>().cullingMask = cullingMask;
            GetComponent<Camera>().clearFlags = clearFlags;
            GetComponent<Camera>().targetTexture = backupTexture;
            GetComponent<Camera>().depth = backupDepth;
            if (m_backupColor)
            {
                GetComponent<Camera>().backgroundColor = backupColor;
            }
        }
    }
}

public class FXCompGlowRender : MonoBehaviour {

    public Shader m_combineShader;

    public Camera m_glowCamera;

    public Material m_combineMaterial;

    void CreateShaderAndMaterial()
    {
        m_combineMaterial = new Material(m_combineShader);
        m_combineMaterial.hideFlags = HideFlags.DontSave;
    }
    // Use this for initialization
    void Start()
    {
        if (m_combineShader == null)
        {
            return;
        }
        int glowLayer = LayerMask.NameToLayer("EffectGlow");
        GameObject obj = new GameObject("FXCompGlowRender_Camera");
        m_glowCamera = obj.AddComponent<Camera>();
        //m_glowCamera.clearFlags = CameraClearFlags.Skybox;
        m_glowCamera.clearFlags = CameraClearFlags.Nothing;
        m_glowCamera.cullingMask = 1<<glowLayer;
        m_glowCamera.depth = GetComponent<Camera>().depth + 20;
        CfgGlowCamera cloneCfg = obj.AddComponent<CfgGlowCamera>();
        cloneCfg.m_cloneSource = this.GetComponent<Camera>();
        cloneCfg.m_backupColor = true;
        cloneCfg.m_manualClearDepth = true;
        //Camera.main = this.camera;
        this.GetComponent<Camera>().cullingMask = this.GetComponent<Camera>().cullingMask & (~(1<<glowLayer));
        CreateShaderAndMaterial();
        obj.SetActive(false);
    }
    void OnPreRender()
    {
        CfgGlowCamera cfg = m_glowCamera.GetComponent<CfgGlowCamera>();
        cfg.m_mainDepthBuffer = Graphics.activeDepthBuffer;
    }
    RenderTexture m_glowTexture;
    public Mesh m_testMesh;
	void OnPostRender()
    {
        if (m_glowTexture == null)
        {
            //m_glowTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
            m_glowTexture = new RenderTexture(Screen.width, Screen.height, 24);
            m_glowTexture.Create();
        }
        RenderBuffer[] rb = new RenderBuffer[] { m_glowTexture.colorBuffer };
        GetComponent<Camera>().SetTargetBuffers(rb, Graphics.activeDepthBuffer);
        RenderBuffer bck = Graphics.activeColorBuffer;
        Graphics.SetRenderTarget(m_glowTexture.colorBuffer, Graphics.activeDepthBuffer);
        GL.Clear(false, true, new Color(0.2f, 0, 0, 0));
        if (m_testMesh != null)
        {
            Graphics.DrawMeshNow(m_testMesh, Matrix4x4.identity);
        }
        m_glowCamera.Render();
        Graphics.SetRenderTarget(bck, Graphics.activeDepthBuffer);
	}
    public bool isInitTexture;
    // Update is called once per frame
    void Update()
    {
        //if (m_glowCamera != null && m_glowCamera.targetTexture == null && isInitTexture)
        //{
        //    isInitTexture = false;
        //    m_glowCamera.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        //}
        CfgGlowCamera.Follow(GetComponent<Camera>(), m_glowCamera);
    }
    // Called by the camera to apply the image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_glowTexture == null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        m_combineMaterial.SetTexture("_Bloom", m_glowTexture);
        Graphics.Blit(source, destination, m_combineMaterial);
    }
}
