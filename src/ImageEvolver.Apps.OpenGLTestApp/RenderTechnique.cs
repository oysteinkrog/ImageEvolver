using Koeky3D.BufferHandling;
using Koeky3D.Shaders;

namespace ImageEvolver.Apps.OpenGLTestApp
{
    internal class RenderTechnique : Technique
    {
        private int _textureLocation;

        public override void Enable()
        {
            // Set the texture variable to 0
            // By default it should be zero, but just to be sure :P
            shader.SetVariable(_textureLocation, 0);
        }

        public override bool Initialise()
        {
            // Load the shaders from a file
            if (!CreateShaderFromFile("Shaders/vertexShader.txt", "Shaders/fragmentShader.txt", ""))
            {
                return false;
            }

            // Set shader attributes, this is where the data will go in the shader
            SetShaderAttribute((int) BufferAttribute.Vertex, "in_Vertex");
            SetShaderAttribute((int) BufferAttribute.TexCoord, "in_TexCoord");
            SetShaderAttribute((int) BufferAttribute.Normal, "in_Normal");

            // Finalize creation of the shader program
            if (!base.Finalize())
            {
                return false;
            }

            // Retrieve the location of the uniforms
            _textureLocation = base.GetUniformLocation("texture");

            // Initialisation was succesful
            return true;
        }
    }
}