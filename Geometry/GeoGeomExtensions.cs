#region copyright
// Copyright (C) Daniel Fink 2019 - All Rights Reserved.
// Unauthorized copying of this file (via any medium) is strictly prohibited.
// Proprietary and confidential.
// Written by Daniel Fink <daniel@placeful.io>, 2018.
#endregion copyright
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;
using Rhino.Geometry.Intersect;


using ProjNet.CoordinateSystems;

using NetTopologySuite.Geometries;

namespace Antelope
{
    /// <summary>
    /// Helper methods for the Geospatial Library
    /// </summary>
    public static class GeoGeomExtensions
    {
        /// <summary>
        /// Orients a curve CCW with relation to XY Plane
        /// </summary>
        public static void OrientCounterClockwise(this Curve curve)
        {
            if (curve.ClosedCurveOrientation(Plane.WorldXY.ZAxis) == CurveOrientation.Clockwise)
            {
                curve.Reverse();
            }
        }

        /// <summary>
        /// Orents a curve CW with relation to XY Plane
        /// </summary>
        public static void OrientClockwise(this Curve curve)
        {
            if (curve.ClosedCurveOrientation(Plane.WorldXY.ZAxis) == CurveOrientation.CounterClockwise)
            {
                curve.Reverse();
            }
        }

        /// <summary>
        /// Extracts a Polyline underlying a Rhino Curve
        /// </summary>
        public static Polyline ExtractPolyline(this Curve curve)
        {
            Polyline polyline;
            var response = curve.TryGetPolyline(out polyline);
            if (response == false)
            {
                throw new WarningException("Warning: Geometry underlying Curve is not a Polyline");
            }
            return polyline;
        }

        /// <summary>
        /// Retrieves the Polyline defining a Brep's boundary
        /// </summary>
        public static Polyline ExtractBoundaryPolyline(this Brep brep)
        {
            var outerCurve = brep.Loops.First(loop => loop.LoopType == BrepLoopType.Outer).To3dCurve();
            return outerCurve.ExtractPolyline();
        }

        /// <summary>
        /// Retrieves the Polylines defining a Brep's holes
        /// </summary>
        public static Polyline[] ExtractHolePolylines(this Brep brep)
        {
            var holePolys = new List<Polyline>();
            var holes = brep.Loops.Where(loop => loop.LoopType == BrepLoopType.Inner);
            if (holes.Any())
            {
                foreach (var holeCurve in holes.Select(brepLoop => brepLoop.To3dCurve()))
                {
                    holePolys.Add(holeCurve.ExtractPolyline());
                }
            }
            return holePolys.ToArray();
        }

        /// <summary>
        /// Cleans a Brep by repairing, standardizing, and compacting it, with options to cap holes and merge faces
        /// </summary>
        public static Brep Clean(this Brep brep, bool capHoles, bool mergeFaces)
        {
            var tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var duplicate = brep.DuplicateBrep();

            try
            {
                if (capHoles)
                {
                    duplicate = duplicate.CapPlanarHoles(tolerance);
                }
                if (mergeFaces)
                {
                    duplicate.MergeCoplanarFaces(tolerance);
                }
                duplicate.Faces.ShrinkFaces();
                duplicate.Repair(tolerance);
                duplicate.Standardize();
                duplicate.Compact();

                if (duplicate.IsValid == false)
                {
                    throw new Exception("Brep Cleaning resulted in an invalid Brep");
                }
            }

            catch (Exception)
            {
                duplicate = null;
            }

            return duplicate;
        }

        /// <summary>
        /// Returns Curve Intersection Events when a curve is self-intersecting
        /// </summary>
        public static CurveIntersections IsSelfIntersecting(this Curve curve, double tolerance)
        {
            var intersections = Intersection.CurveCurve(curve, curve, tolerance, tolerance);
            return intersections;
        }

        /// <summary>
        /// Cleans a Polyline by performing a repeat-offset using Clipper
        /// </summary>
        //public static Polyline[] ClipperClean(this Polyline polyline, double tolerance)
        //{
        //    var polylines = new List<Polyline>();

        //    var inputPolyline = new List<Polyline> { polyline };
        //    List<Polyline> contours;
        //    List<Polyline> holes;
        //    var openFilletType = Polyline3D.OpenFilletType.Butt;
        //    var closedFilletType = Polyline3D.ClosedFilletType.Miter;
        //    Plane plane;
        //    Plane.FitPlaneToPoints(polyline.ToList(), out plane);

        //    Polyline3D.Offset(inputPolyline, openFilletType, closedFilletType, tolerance, plane, 1e-12, out contours, out holes);

        //    foreach (Polyline hole in holes)
        //    {
        //        List<Polyline> holeContours;
        //        List<Polyline> holeHoles;

        //        Polyline3D.Offset(new List<Polyline> { hole }, openFilletType, closedFilletType, tolerance, plane, 1e-12, out holeContours, out holeHoles);
        //        foreach (Polyline holeContour in holeContours)
        //        {
        //            holeContour.ReduceSegments(tolerance);
        //            polylines.Add(holeContour);
        //        }
        //    }

        //    return polylines.ToArray();
        //}
    }
}
