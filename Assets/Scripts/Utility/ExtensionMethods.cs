using UnityEngine;
using System.Collections;

namespace ExtensionMethods
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Uniquely identify a GameObject within the Scene
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>names of the gameobject and it's ancestors seperated by forward slashes / </returns>
        public static string PathID(this GameObject subject)
        {
            if (subject.transform.parent == null) return subject.name;

            return subject.transform.parent.gameObject.PathID() + "/" + subject.name;
        }

        /// <summary>
        /// Find a GameObject given it's PathID
        /// </summary>
        /// <param name="utility"></param>
        /// <param name="path">PathID to the subject GameObject</param>
        /// <returns>null if the path cannot be resolved or the specified GameObject</returns>
        public static GameObject FindByPathID(this GameObject utility, string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var names = path.Split('/');
            int index = 1;
            var rootGameObject = GameObject.Find(names[0]);

            if (rootGameObject == null) return null;

            var result = rootGameObject.transform;

            while (index < names.Length && result != null)
            {
                result = result.transform.Find(names[index]);
                index++;
            }

            if (result == null) return null;

            return result.gameObject;
        } //end of FindByPathID

        /// <summary>
        /// Uniquely identify an instance of a component in a scene
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static string PathID (this Component subject)
        {
            return subject.gameObject.PathID() + "." + subject.GetType().Name;
        }

        /// <summary>
        /// muate some part of a vector 3 and return the resulting vector
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Mutate(this Vector3 subject, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3()
            {
                x = x ?? subject.x,
                y = y ?? subject.y,
                z = z ?? subject.z
            };
        }

        /// <summary>
        /// Find the pitch and yaw from this transform to another transform
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>yaw on the x axis, pitch on the y axis in degrees</returns>
        public static Vector3 PitchAndYaw(this Transform start, Transform end)
        {
            return PitchAndYaw(start, end.position);
        }

        /// <summary>
        /// Find the pitch and yaw from this transform to an end position
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>yaw on the x axis, pitch on the y axis in degrees</returns>
        public static Vector3 PitchAndYaw(this Transform start, Vector3 end)
        {
            Vector3 localEndPos = start.InverseTransformPoint(end);
            float range = Vector3.Distance(start.position, end);
            float yaw = Mathf.Tan(localEndPos.x / localEndPos.z) * Mathf.Rad2Deg;
            float pitch = Mathf.Sin(localEndPos.y / range) * Mathf.Rad2Deg;

            while (yaw < -179f) yaw = yaw + 360f;
            while (yaw > 180f) yaw = yaw - 360f;
            while (pitch < -179f) pitch = pitch + 360f;
            while (pitch > 180f) pitch = pitch - 360f;

            //while (yaw < 0f) yaw = yaw + 360f;
            //while (yaw > 360f) yaw = yaw - 360f;
            //while (pitch < 0f) pitch = pitch + 360f;
            //while (pitch > 360f) pitch = pitch - 360f;

            return new Vector3(yaw, pitch, range);
        }

        // turns "the cat sat on the mat"  to "The Cat Sat On The Mat"
        public static string ToTitleCase(this string subject)
        {
            if (string.IsNullOrEmpty(subject)) return string.Empty;

            var buffer = subject.ToCharArray();
            bool previousCharIsWhitespace = true;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (previousCharIsWhitespace)
                {
                    buffer[i] = char.ToUpper(buffer[i]);
                }
                else
                {
                    buffer[i] = char.ToLower(buffer[i]);
                }
                previousCharIsWhitespace = char.IsWhiteSpace(buffer[i]);
            }

            return new string(buffer);
        } //end ToTitleCase

        // turns "the cat sat on the mat"  to "TheCatSatOnTheMat"
        public static string ToPascalCase(this string subject)
        {
            if (string.IsNullOrEmpty(subject)) return string.Empty;

            var buffer = new System.Text.StringBuilder();
            var temp = subject.ToTitleCase().ToCharArray();
            foreach(char c in temp)
            {
                if (!char.IsWhiteSpace(c)) buffer.Append(c);
            }

            return buffer.ToString();
        } //end ToPascalCase

        // turns "the cat sat on the mat"  to "theCatSatOnTheMat"
        public static string ToCamelCase(this string subject)
        {
            if (string.IsNullOrEmpty(subject)) return string.Empty;

            var buffer = subject.ToPascalCase().ToCharArray();

            buffer[0] = char.ToLower(buffer[0]);

            return new string(buffer);
        } //end ToCamelCase

        public static string UnCamelCase(this string subject)
        {
            if (string.IsNullOrEmpty(subject)) return string.Empty;

            var buffer = new System.Text.StringBuilder();
            bool prevCharIsLowerCase = false;

            foreach(char c in subject.ToCharArray())
            {
                if(char.IsLower(c))
                {
                    buffer.Append(c);
                    prevCharIsLowerCase = true;
                } else
                {
                    if (prevCharIsLowerCase)
                    {
                        buffer.Append(" ");
                    }
                    buffer.Append(c);
                    prevCharIsLowerCase = false;
                }
            }

            return buffer.ToString();
        } //end UnCamelCase
    }
}