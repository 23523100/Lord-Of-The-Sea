using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RenderTargetIdentifier source;
        private Material _material;
        private int tempColorId;
        private int tempDepthId;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
            tempColorId = Shader.PropertyToID("_TemporaryColourTexture");
            tempDepthId = Shader.PropertyToID("_TemporaryDepthTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) { }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            var cmd = CommandBufferPool.Get("Water_Volume_Pass");

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempColorId, desc, FilterMode.Bilinear);

            cmd.Blit(source, new RenderTargetIdentifier(tempColorId), _material);
            cmd.Blit(new RenderTargetIdentifier(tempColorId), source);

            cmd.ReleaseTemporaryRT(tempColorId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd) { }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();
    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
            settings.material = (Material)Resources.Load("Water_Volume");

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Gunakan cameraColorTargetHandle di Unity 6.2 (jangan pakai cameraColorTarget yang obsolete)
        var camHandle = renderer.cameraColorTargetHandle;
        m_ScriptablePass.source = camHandle; // implicit conversion untuk RTHandle -> RenderTargetIdentifier
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
