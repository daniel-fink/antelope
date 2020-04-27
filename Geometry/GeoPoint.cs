#region copyright
// Copyright (C) Daniel Fink 2019 - All Rights Reserved.
// Unauthorized copying of this file (via any medium) is strictly prohibited.
// Proprietary and confidential.
// Written by Daniel Fink <daniel@placeful.io>, 2018.
#endregion copyright
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using ProjNet.CoordinateSystems;

using NetTopologySuite.Geometries;

namespace Antelope
{
    /// <summary>
    /// A Vertex with geospatial positioning information.
    /// </summary>
    public class GeoPoint : Rhino.Geometry.Point, IGeospatialGeom
    {
        public ProjectionSystem ProjectionSystem { get; private set; }

        public GeoPoint(Point3d point, ProjectionSystem sourceProjection) : base(point)
        {
            this.Location = point;
            this.ProjectionSystem = sourceProjection;
        }

        public GeoPoint(NetTopologySuite.Geometries.Point point, Plane sourceBasis) : base(point.Coordinate.ToPoint3d())
        {
            this.Location = point.Coordinate.ToPoint3d();
            //this.Geometry = this.GetGeometry();
            this.ProjectionSystem = new ProjectionSystem(point.SRID, sourceBasis);
        }

        private Transform TargetTransform()
        {
            return Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, this.ProjectionSystem.SourceBasis);
        }

        /// <summary>
        /// Transforms an GeoPoint from its source basis to Rhino's origin basis (target).
        /// </summary>
        public void ToTargetBasis()
        {
            this.Transform(this.TargetTransform());
        }

        /// <summary>
        /// Transforms an GeoPoint from its target basis to its source basis.
        /// </summary>
        public void ToSourceBasis()
        {
            Transform inverseTransform;
            this.TargetTransform().TryGetInverse(out inverseTransform);
            this.Transform(inverseTransform);
        }
    }

    /// <summary>
    /// Multiple Vertices with geospatial positioning information.
    /// </summary>
    // [Serializable]
    // public class MultiGeoPoint : IGeospatialGeom
    // {
    //     public ProjectionSystem ProjectionSystem { get; private set; }
    //     public GeometryBase[] Geometries
    //     {
    //         get
    //         {
    //             return this.GetGeometries();
    //         }
    //         private set
    //         {
    //
    //         }
    //     }
    //
    //     private GeometryBase[] GetGeometries()
    //     {
    //         var geoms = this.Objects.Select(geom => (GeoPoint)geom);
    //         return geoms.Select(geom => (GeometryBase)geom).ToArray();
    //     }
    //
    //     public void Add(GeoPoint geoPoint)
    //     {
    //         this.Objects.Add(geoPoint);
    //     }
    //
    //     public MultiGeoPoint(IEnumerable<GeoPoint> points)
    //     {
    //         foreach (var point in points)
    //         {
    //             this.Objects.Add(point);
    //         }
    //         var projSystems = points.Select(p => p.ProjectionSystem);
    //         this.ProjectionSystem = projSystems.Condense<ProjectionSystem>();
    //         this.Geometries = this.GetGeometries();
    //     }
    //
    //     public MultiGeoPoint(IEnumerable<Point3d> points, ProjectionSystem sourceProjection)
    //     {
    //         foreach (var point in points)
    //         {
    //             this.Objects.Add(new GeoPoint(point, sourceProjection));
    //         }
    //         this.ProjectionSystem = sourceProjection;
    //         this.Geometries = this.GetGeometries();
    //     }
    //
    //     public MultiGeoPoint(IEnumerable<NetTopologySuite.Geometries.Point> points, Plane sourceBasis)
    //     {
    //         foreach (var point in points)
    //         {
    //             this.Objects.Add(new GeoPoint(point, sourceBasis));
    //         }
    //         var srids = points.Select(ls => ls.SRID);
    //         this.ProjectionSystem = new ProjectionSystem(srids.Condense<int>(), sourceBasis);
    //         this.Geometries = this.GetGeometries();
    //     }
    //
    //     private Transform TargetTransform()
    //     {
    //         return Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, this.ProjectionSystem.SourceBasis);
    //     }
    //
    //     /// <summary>
    //     /// Transforms an GeoPoint Collection from its source basis to Rhino's origin basis (target).
    //     /// </summary>
    //     public void ToTargetBasis()
    //     {
    //         this.Objects.Select(ls => ls.Transform(this.TargetTransform()));
    //     }
    //
    //     /// <summary>
    //     /// Transforms an GeoPoint Collection from its target basis to its source basis.
    //     /// </summary>
    //     public void ToSourceBasis()
    //     {
    //         Transform inverseTransform;
    //         this.TargetTransform().TryGetInverse(out inverseTransform);
    //         this.Objects.Select(ls => ls.Transform(inverseTransform));
    //     }
    // }
}