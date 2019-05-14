using LibTessDotNet.Double;
using System;
using System.Drawing;

namespace TessExample
{
    class Program
    {
        // The data array contains 4 values, it's the associated data of the vertices that resulted in an intersection.
        private static object VertexCombine(Vec3 position, object[] data, double[] weights)
        {
            // Fetch the vertex data.
            var colors = new Color[] { (Color)data[0], (Color)data[1], (Color)data[2], (Color)data[3] };
            // Interpolate with the 4 weights.
            var rgba = new float[] {
                (float)colors[0].R * (float)weights[0] + (float)colors[1].R * (float)weights[1] + (float)colors[2].R * (float)weights[2] + (float)colors[3].R * (float)weights[3],
                (float)colors[0].G * (float)weights[0] + (float)colors[1].G * (float)weights[1] + (float)colors[2].G * (float)weights[2] + (float)colors[3].G * (float)weights[3],
                (float)colors[0].B * (float)weights[0] + (float)colors[1].B * (float)weights[1] + (float)colors[2].B * (float)weights[2] + (float)colors[3].B * (float)weights[3],
                (float)colors[0].A * (float)weights[0] + (float)colors[1].A * (float)weights[1] + (float)colors[2].A * (float)weights[2] + (float)colors[3].A * (float)weights[3]
            };
            // Return interpolated data for the new vertex.
            return Color.FromArgb((int)rgba[3], (int)rgba[0], (int)rgba[1], (int)rgba[2]);
        }

        static void Main(string[] args)
        {
            // Example input data in the form of a star that intersects itself.
            var inputData = new float[] { 0.0f, 3.0f, -1.0f, 0.0f, 1.6f, 1.9f, -1.6f, 1.9f, 1.0f, 0.0f };

            // Create an instance of the tessellator. Can be reused.
            var tess = new Tess();

            // Construct the contour from inputData.
            // A polygon can be composed of multiple contours which are all tessellated at the same time.
            int numPoints = inputData.Length / 2;
            var contour = new ContourVertex[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                // NOTE : Z is here for convenience if you want to keep a 3D vertex position throughout the tessellation process but only X and Y are important.
                contour[i].Position = new Vec3(inputData[i * 2], inputData[i * 2 + 1], 0);
                // Data can contain any per-vertex data, here a constant color.
                contour[i].Data = Color.Azure;
            }
            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
            tess.AddContour(contour, ContourOrientation.Clockwise);

            // Tessellate!
            // The winding rule determines how the different contours are combined together.
            // See http://www.glprogramming.com/red/chapter11.html (section "Winding Numbers and Winding Rules") for more information.
            // If you want triangles as output, you need to use "Polygons" type as output and 3 vertices per polygon.
            tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3, VertexCombine);

            // Same call but the last callback is optional. Data will be null because no interpolated data would have been generated.
            //tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3); // Some vertices will have null Data in this case.

            Console.WriteLine("Output triangles:");
            int numTriangles = tess.ElementCount;
            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;
                Console.WriteLine("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y);
            }
            Console.ReadLine();
        }
    }
}
