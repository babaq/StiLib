#region File Description
//-----------------------------------------------------------------------------
// SLCamera.cs
//
// StiLib Basic Camera Service.
// Copyright (c) Zhang Li. 2009-02-21.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// Free Camera
    /// </summary>
    public class SLFreeCamera : SLCamera
    {
        #region Fields

        #endregion

        #region Properties

        #endregion


        /// <summary>
        /// Init
        /// </summary>
        public SLFreeCamera()
        {
        }

        /// <summary>
        ///  Update Camera according to Input
        /// </summary>
        /// <param name="input"></param>
        public void Update(SLInput input)
        {
        }

    }

    /// <summary>
    /// StiLib Basic Camera
    /// </summary>
    public class SLCamera : ICloneable
    {
        #region Fields

        /// <summary>
        /// Camera Position in World Space
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Camera Shooting Target in World Space
        /// </summary>
        public Vector3 Target;
        /// <summary>
        /// Camera Space UP Direction in World Space
        /// </summary>
        public Vector3 Up;
        /// <summary>
        /// Camera Current Projection Type
        /// </summary>
        public ProjectionType projtype;
        /// <summary>
        /// Perspective Projection Field of View
        /// </summary>
        public float FoV;
        /// <summary>
        /// Projection Front Clipping Plane
        /// </summary>
        public float NearPlane;
        /// <summary>
        /// Projection Back Clipping Plane
        /// </summary>
        public float FarPlane;
        /// <summary>
        ///  Viewport of Current Camera
        /// </summary>
        public Viewport viewport;

        #endregion

        #region Properties

        /// <summary>
        /// Get Camera Current View Matrix
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return Matrix.CreateLookAt(Position, Target, Up); }
        }

        /// <summary>
        /// Get Camera Current Ununified Projection Matrix
        /// </summary>
        public Matrix ProjMatrix
        {
            get { return GetUnitProj(projtype, 1.0f); }
        }

        /// <summary>
        /// Get Camera Current Shooting Direction
        /// </summary>
        public Vector3 Direction
        {
            get { return Target - Position; }
        }

        #endregion


        /// <summary>
        /// Init to default  -- Position:(0,0,5), Target:(0,0,0), Up:(0,1,0), projtype:Ortho, FoV:PI/2, 
        /// NearPlane:5, FarPlane:200, viewport: default
        /// </summary>
        public SLCamera() : this(new Viewport())
        {
        }

        /// <summary>
        /// Init to default but custom GraphicsDevice's viewport settings
        /// </summary>
        /// <param name="gd"></param>
        public SLCamera(GraphicsDevice gd) : this(gd.Viewport)
        {
        }

        /// <summary>
        /// Init to default  -- Position:(0,0,5), Target:(0,0,0), Up:(0,1,0), projtype:Ortho, FoV:PI/4, 
        /// NearPlane:5, FarPlane:200 and custom viewport settings
        /// </summary>
        /// <param name="vport"></param>
        public SLCamera(Viewport vport) : this(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.Up, ProjectionType.Orthographic, (float)Math.PI / 4, 5.0f, 200.0f, vport)
        {
        }

        /// <summary>
        /// Init Camera to custom settings
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <param name="type"></param>
        /// <param name="fov"></param>
        /// <param name="nearplane"></param>
        /// <param name="farplane"></param>
        /// <param name="vport"></param>
        public SLCamera(Vector3 position, Vector3 target, Vector3 up, ProjectionType type, float fov, float nearplane, float farplane, Viewport vport)
        {
            Position = position;
            Target = target;
            Up = up;
            projtype = type;
            FoV = fov;
            NearPlane = nearplane;
            FarPlane = farplane;
            viewport = vport;
        }


        /// <summary>
        /// Init according to custom GraphicsDevice's viewport
        /// </summary>
        /// <param name="gd"></param>
        public void Init(GraphicsDevice gd)
        {
            Init(gd.Viewport);
        }

        /// <summary>
        /// Init according to custom viewport
        /// </summary>
        /// <param name="vport"></param>
        public void Init(Viewport vport)
        {
            Init(Position, Target, Up, projtype, FoV, NearPlane, FarPlane, vport);
        }

        /// <summary>
        /// Init Camera to custom settings
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <param name="type"></param>
        /// <param name="fov"></param>
        /// <param name="nearplane"></param>
        /// <param name="farplane"></param>
        /// <param name="vport"></param>
        public void Init(Vector3 position, Vector3 target, Vector3 up, ProjectionType type, float fov, float nearplane, float farplane, Viewport vport)
        {
            Position = position;
            Target = target;
            Up = up;
            projtype = type;
            FoV = fov;
            NearPlane = nearplane;
            FarPlane = farplane;
            viewport = vport;
        }

        /// <summary>
        /// Get Camera Unified Projection Matrix According to Custom Projection Type and Unit Factor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uf"></param>
        /// <returns></returns>
        public Matrix GetUnitProj(ProjectionType type, float uf)
        {
            if (type == ProjectionType.Perspective)
                return Matrix.CreatePerspectiveFieldOfView(FoV, viewport.AspectRatio, NearPlane, FarPlane);
            else if (type == ProjectionType.Orthographic)
                return Matrix.CreateOrthographic(viewport.Width / uf, viewport.Height / uf, NearPlane, FarPlane);

            return Matrix.Identity;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            SLCamera cam = new SLCamera();
            cam.Init(Position, Target, Up, projtype, FoV, NearPlane, FarPlane, viewport);
            return cam;
        }

    }

    /// <summary>
    /// Projection Type
    /// </summary>
    public enum ProjectionType
    {
        /// <summary>
        /// Perspective Projection
        /// </summary>
        Perspective,
        /// <summary>
        /// Orthographic Projection
        /// </summary>
        Orthographic
    }

}
