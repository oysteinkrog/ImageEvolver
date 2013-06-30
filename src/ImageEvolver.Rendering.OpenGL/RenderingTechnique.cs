#region Copyright

//     ImageEvolver
//     Copyright (C) 2013-2013 Øystein Krog
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using Koeky3D.BufferHandling;
using Koeky3D.Shaders;

namespace ImageEvolver.Rendering.OpenGL
{
    public class RenderingTechnique : Technique
    {
        private const string VertexShader = @"
#version 140
in vec3 in_Vertex;
in vec4 in_Color;

// output
out vec4 out_Color;

uniform mat4x4 projection, view, world;

void main()
{
    // pass color through to fragment shader
    out_Color = in_Color;

    gl_Position = projection * view * world * vec4(in_Vertex, 1.0f);
}";

        private const string FragmentShader = @"
#version 140
in vec4 out_Color;

void main()
{
    gl_FragColor = out_Color;
}";

        /// <summary>
        ///     Enables this technique
        /// </summary>
        public override void Enable() {}

        /// <summary>
        ///     Initialises this technique
        /// </summary>
        /// <returns>True if the technique succesfully initialised</returns>
        public override bool Initialise()
        {
            if (!CreateShaderFromSource(VertexShader, FragmentShader, ""))
            {
                return false;
            }

            // Set shader attributes
            SetShaderAttribute((int) BufferAttribute.Vertex, "in_Vertex");
            SetShaderAttribute((int) BufferAttribute.Color, "in_Color");

            if (!Finalize())
            {
                return false;
            }

            return true;
        }
    }
}