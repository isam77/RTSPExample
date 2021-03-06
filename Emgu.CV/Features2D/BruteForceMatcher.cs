﻿//----------------------------------------------------------------------------
//  Copyright (C) 2004-2012 by EMGU. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.Util;

namespace Emgu.CV.Features2D
{
   /// <summary>
   /// The match distance type
   /// </summary>
   public enum DistanceType
   {
      /// <summary>
      /// 
      /// </summary>
      Inf = 1,
      /// <summary>
      /// Manhattan distance (city block distance)
      /// </summary>
      L1 = 2,
      /// <summary>
      /// Squared Euclidean distance
      /// </summary>
      L2 = 4, 
      /// <summary>
      /// Euclidean distance
      /// </summary>
      L2Sqr = 5,
      /// <summary>
      /// Hamming distance functor - counts the bit differences between two strings - useful for the Brief descriptor, 
      /// bit count of A exclusive XOR'ed with B. 
      /// </summary>
      Hamming = 6,
      /// <summary>
      /// Hamming distance functor - counts the bit differences between two strings - useful for the Brief descriptor, 
      /// bit count of A exclusive XOR'ed with B. 
      /// </summary>
      Hamming2 = 7, //TODO: update the documentation
      /*
      TypeMask = 7, 
      Relative = 8, 
      MinMax = 32 */
   }

   /// <summary>
   /// Wrapped BruteForceMatcher
   /// </summary>
   /// <typeparam name="T">The type of data to be matched. Can be either float or Byte</typeparam>
   public class BruteForceMatcher<T> : UnmanagedObject
      where T : struct
   {
      /// <summary>
      /// Find the k-nearest match
      /// </summary>
      /// <param name="queryDescriptor">An n x m matrix of descriptors to be query for nearest neighbours. n is the number of descriptor and m is the size of the descriptor</param>
      /// <param name="trainIdx">The resulting n x <paramref name="k"/> matrix of descriptor index from the training descriptors</param>
      /// <param name="distance">The resulting n x <paramref name="k"/> matrix of distance value from the training descriptors</param>
      /// <param name="k">Number of nearest neighbors to search for</param>
      /// <param name="mask">Can be null if not needed. An n x 1 matrix. If 0, the query descriptor in the corresponding row will be ignored.</param>
      public void KnnMatch(Matrix<T> queryDescriptor, Matrix<int> trainIdx, Matrix<float> distance, int k, Matrix<Byte> mask)
      {
         CvInvoke.CvDescriptorMatcherKnnMatch(Ptr, queryDescriptor, trainIdx, distance, k, mask);
      }

      private DistanceType _distanceType;

      /// <summary>
      /// Create a BruteForceMatcher of the specific distance type, without cross check.
      /// </summary>
      /// <param name="distanceType">The distance type</param>
      public BruteForceMatcher(DistanceType distanceType)
         : this (distanceType, false)
      {
      }

      /// <summary>
      /// Create a BruteForceMatcher of the specific distance type
      /// </summary>
      /// <param name="distanceType">The distance type</param>
      /// <param name="crossCheck">Specify whether or not cross check is needed. Use false for default.</param>
      public BruteForceMatcher(DistanceType distanceType, bool crossCheck)
      {
         if (distanceType == DistanceType.Hamming || distanceType == DistanceType.Hamming2)
         { 
            if (typeof(T) != typeof(byte))
               throw new ArgumentException("Hamming distance type requires model descriptor to be Matrix<Byte>");
         }

         if (typeof(T) != typeof(byte) && typeof(T) != typeof(float))
         {
            throw new NotImplementedException(String.Format("Data type of {0} is not supported", typeof(T).ToString()));
         }

         _distanceType = distanceType;
         _ptr = CvInvoke.CvBruteForceMatcherCreate(_distanceType, crossCheck);
      }

      /// <summary>
      /// Add the model descriptors
      /// </summary>
      /// <param name="modelDescriptors">The model discriptors</param>
      public void Add(Matrix<T> modelDescriptors)
      {
         CvInvoke.CvDescriptorMatcherAdd(_ptr, modelDescriptors);
      }

      /// <summary>
      /// Release the unmanaged resource associated with the BruteForceMatcher
      /// </summary>
      protected override void DisposeObject()
      {
         CvInvoke.CvBruteForceMatcherRelease(ref _ptr);
      }
   }
}

namespace Emgu.CV
{
   public static partial class CvInvoke
   {
      [DllImport(CvInvoke.EXTERN_LIBRARY, CallingConvention = CvInvoke.CvCallingConvention)]
      internal extern static IntPtr CvBruteForceMatcherCreate(
         Features2D.DistanceType distanceType,
         [MarshalAs(CvInvoke.BoolMarshalType)]
         bool crossCheck);

      [DllImport(CvInvoke.EXTERN_LIBRARY, CallingConvention = CvInvoke.CvCallingConvention)]
      internal extern static void CvBruteForceMatcherRelease(ref IntPtr matcher);

      [DllImport(CvInvoke.EXTERN_LIBRARY, CallingConvention = CvInvoke.CvCallingConvention)]
      internal extern static void CvDescriptorMatcherAdd(IntPtr matcher, IntPtr trainDescriptor);

      [DllImport(CvInvoke.EXTERN_LIBRARY, CallingConvention = CvInvoke.CvCallingConvention)]
      internal extern static void CvDescriptorMatcherKnnMatch(IntPtr matcher, IntPtr queryDescriptors,
                   IntPtr trainIdx, IntPtr distance, int k,
                   IntPtr mask);
   }
}