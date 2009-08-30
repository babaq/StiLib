#region File Description
//-----------------------------------------------------------------------------
// SLCamera.cs
//
// StiLib Camera Service.
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
    /// StiLib Free Camera
    /// </summary>
    public class SLFreeCamera : SLCamera
    {
        #region Fields

        #endregion

        #region Properties

        #endregion


        /// <summary>
        /// Init with Default
        /// </summary>
        public SLFreeCamera()
        {
        }

        /// <summary>
        ///  Update Camera According to User Input
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
        public ProjectionType projectionType;
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
        public Matrix ProjectionMatrix
        {
            get { return GetUnitProjection(projectionType, 1.0f); }
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
        /// Init with Default -- Position: (0, 0, 5), Target: (0, 0, 0), Up: (0, 1, 0), projectiontype: Ortho, FoV: PI/4, 
        /// NearPlane: 5, FarPlane: 200, viewport: default
        /// </summary>
        public SLCamera()
            : this(new Viewport())
        {
        }

        /// <summary>
        /// Init with Current GraphicsDevice Viewport and Default -- Position: (0, 0, 5), Target: (0, 0, 0), Up: (0, 1, 0), projectiontype: Ortho, FoV: PI/4, 
        /// NearPlane: 5, FarPlane: 200
        /// </summary>
        /// <param name="gd"></param>
        public SLCamera(GraphicsDevice gd)
            : this(gd.Viewport)
        {
        }

        /// <summary>
        /// Init with Custom Viewport and Default -- Position: (0, 0, 5), Target: (0, 0, 0), Up: (0, 1, 0), projectiontype: Ortho, FoV: PI/4, 
        /// NearPlane: 5, FarPlane: 200
        /// </summary>
        /// <param name="viewport"></param>
        public SLCamera(Viewport viewport)
            : this(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.Up, ProjectionType.Orthographic, (float)Math.PI / 4, 5.0f, 200.0f, viewport)
        {
        }

        /// <summary>
        /// Init Camera with Custom parameters
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <param name="projtype"></param>
        /// <param name="fov"></param>
        /// <param name="nearplane"></param>
        /// <param name="farplane"></param>
        /// <param name="viewport"></param>
        public SLCamera(Vector3 position, Vector3 target, Vector3 up, ProjectionType projtype, float fov, float nearplane, float farplane, Viewport viewport)
        {
            SetCamera(position, target, up, projtype, fov, nearplane, farplane, viewport);
        }


        /// <summary>
        /// Set Only Camera's Viewport According to Current GraphicsDevice Viewport
        /// </summary>
        /// <param name="gd"></param>
        public void SetCamera(GraphicsDevice gd)
        {
            SetCamera(gd.Viewport);
        }

        /// <summary>
        /// Set Only Camera's Viewport
        /// </summary>
        /// <param name="viewport"></param>
        public void SetCamera(Viewport viewport)
        {
            SetCamera(Position, Target, Up, projectionType, FoV, NearPlane, FarPlane, viewport);
        }

        /// <summary>
        /// Set Camera Parameters
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        /// <param name="projtype"></param>
        /// <param name="fov"></param>
        /// <param name="nearplane"></param>
        /// <param name="farplane"></param>
        /// <param name="viewport"></param>
        public void SetCamera(Vector3 position, Vector3 target, Vector3 up, ProjectionType projtype, float fov, float nearplane, float farplane, Viewport viewport)
        {
            this.Position = position;
            this.Target = target;
            this.Up = up;
            this.projectionType = projtype;
            this.FoV = fov;
            this.NearPlane = nearplane;
            this.FarPlane = farplane;
            this.viewport = viewport;
        }

        /// <summary>
        /// Get Camera Unified Projection Matrix According to Custom Projection Type and Unit Factor
        /// </summary>
        /// <param name="projtype"></param>
        /// <param name="unitfactor"></param>
        /// <returns></returns>
        public Matrix GetUnitProjection(ProjectionType projtype, float unitfactor)
        {
            if (projtype == ProjectionType.Perspective)
                return Matrix.CreatePerspectiveFieldOfView(FoV, viewport.AspectRatio, NearPlane, FarPlane);
            else if (projtype == ProjectionType.Orthographic)
                return Matrix.CreateOrthographic(viewport.Width / unitfactor, viewport.Height / unitfactor, NearPlane, FarPlane);

            return Matrix.Identity;
        }

        /// <summary>
        /// Copy Current Instance and Change to Current GraphicsDevice Viewport
        /// </summary>
        /// <param name="gd"></param>
        /// <returns></returns>
        public object Clone(GraphicsDevice gd)
        {
            SLCamera cam = new SLCamera();
            cam.SetCamera(Position, Target, Up, projectionType, FoV, NearPlane, FarPlane, gd.Viewport);
            return cam;
        }

        /// <summary>
        /// Clone Current Instance and Change to Custom Viewport
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public object Clone(Viewport viewport)
        {
            return new SLCamera(Position, Target, Up, projectionType, FoV, NearPlane, FarPlane, viewport);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new SLCamera(Position, Target, Up, projectionType, FoV, NearPlane, FarPlane, viewport);
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
