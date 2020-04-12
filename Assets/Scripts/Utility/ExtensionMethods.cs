using UnityEngine;
using System.Collections;
using System;

namespace ExtensionMethods
{
    public static class UnityExtensions
    {
        #region "gameobject or component by it's path in the scene heirarchy"
        /// <summary>
        /// Uniquely identify a GameObject within the Scene
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>names of the gameobject and it's ancestors seperated by forward slashes / </returns>
        /// <remarks>as object names are no required to be unique and may contain forward slashes uniqueness is not garunteed</remarks>
        public static string PathID(this GameObject subject)
        {
            if (subject.transform.parent == null) return subject.name;

            return subject.transform.parent.gameObject.PathID() + "/" + subject.name;
        }

        /// <summary>
        /// Uniquely identify an instance of a component in a scene
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>a string to identify an instance of a component in a scene</returns>
        /// <remarks>as object names are no required to be unique and a game object may have several instances of the same component uniqueness is not garunteed</remarks>
        public static string PathID(this Component subject)
        {
            return subject.gameObject.PathID() + "." + subject.GetType().Name;
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
        /// Find a Component given it's PathID
        /// </summary>
        /// <typeparam name="T">Type of component expected</typeparam>
        /// <param name="utility"></param>
        /// <param name="path">PathID to the desired component</param>
        /// <returns>null if the path cannot be resolved or the specified Component cannot be found</returns>
        public static T FindByPathID<T>(this GameObject utility, string path) where T : Component
        {
            if (string.IsNullOrEmpty(path))
                return null;

            int lastFullStop = path.LastIndexOf('.');

            if (lastFullStop == -1)
                return null;

            var componentTypeName = path.Substring(lastFullStop);
            path = path.Substring(0, lastFullStop);
            GameObject foundGameObject = utility.FindByPathID(path);

            if (foundGameObject == null)
                return null;

            return foundGameObject.GetComponent<T>();
        }

        /// <summary>
        /// Get the nearest parent GameObject with a given tag
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="tag">tag being sought</param>
        /// <returns>a gameombect that has the tag or null if no match was found</returns>
        public static GameObject GetParentWithTag(this GameObject subject, string tag)
        {
            if (tag == null)
                return null;

            var parentTransform = subject.transform.parent;

            if (parentTransform == null)
                return null;

            if (parentTransform.CompareTag(tag))
                return parentTransform.gameObject;

            return parentTransform.gameObject.GetParentWithTag(tag);
        }

        /// <summary> Checking whether the layer belongs to the current mask </summary>
        /// <returns> Return true if this maks contains the layer</returns>
        /// lifted from a 2D shooter sample project, thx mate :D
        public static bool Contains(this LayerMask mask, int layer)
        {
            return ((mask.value & (1 << layer)) != 0);
        }

        /// <summary>
        /// Checking if this mask contains the layer specified by it's name
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layerName">name of a layer in the scene</param>
        /// <returns>Return true if this maks contains the layer</returns>
        public static bool Contains(this LayerMask mask, string layerName)
        {
            if (string.IsNullOrEmpty(layerName))
                return false;

            int layer = LayerMask.NameToLayer(layerName);

            if (layer == -1)
                return false;

            return mask.Contains(layer);
        }

        #endregion

        /// <summary>
        /// muate some part of a vector 3 and return the resulting vector
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>a modified form of the original vector, where any non-null paramter replace their respective axis</returns>
        /// <example>var v1 = Vector3.zero; v1.Mutate( y:1 ); //result v1 = {0,1,0} </example>
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
        /// Compare this vector to another
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="other"></param>
        /// <returns>true if all axis are approximatly the same</returns>
        public static bool Approximately(this Vector3 subject, Vector3 other)
        {
            return Mathf.Approximately(subject.x, other.x) 
                && Mathf.Approximately(subject.y, other.y) 
                && Mathf.Approximately(subject.z, other.z);
        }

        /// <summary> Find direction to Vector2 </summary>
        /// <returns> Return direction with magnitude = 1 </returns>
        public static Vector2 GetDirectionTo(this Vector2 pos, Vector2 directionPos)
        {
            var direction = directionPos - pos;

            if (direction.sqrMagnitude == 0)
            {
                return Vector2.zero;
            }

            direction.Normalize();

            return direction;
        }

        /// <summary> Find direction to Vector3 </summary>
        /// <returns> Return direction with magnitude = 1 </returns>
        public static Vector3 GetDirectionTo(this Vector3 pos, Vector3 directionPos)
        {
            var direction = directionPos - pos;

            if (direction.sqrMagnitude == 0)
            {
                return Vector3.zero;
            }

            direction.Normalize();

            return direction;
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

        //untested
        public static Rect WorldRect(this RectTransform _rectTransform)
        {
            var localRect = _rectTransform.rect;
            var worldPos = _rectTransform.TransformPoint(localRect.position);
            var worldSize = _rectTransform.TransformVector(localRect.width, localRect.height, 0f);
            var result = new Rect()
            {
                position = worldPos,
                width = worldSize.x,
                height = worldSize.y
            };

            return result;
        }

        #region "convert between Vector types"
        public static Vector2 ToVector2(this Vector2Int source)
        {
            return new Vector2()
            {
                x = (float)source.x,
                y = (float)source.y
            };
        }

        public static Vector3 ToVector3(this Vector2 source, float z = 0f)
        {
            return new Vector3()
            {
                x = source.x,
                y = source.y,
                z = z
            };
        }

        public static Vector3 ToVector3(this Vector2Int source, float z = 0f)
        {
            return new Vector3()
            {
                x = source.x,
                y = source.y,
                z = z
            };
        }

        public static Vector2Int ToVector2Int(this Vector3 source)
        {
            return new Vector2Int()
            {
                x = Mathf.RoundToInt(source.x),
                y = Mathf.RoundToInt(source.y)
            };
        }

        public static Vector2 ToVector2(this Vector3 source)
        {
            return new Vector2()
            {
                x = (float)source.x,
                y = (float)source.y
            };
        }

        /// <summary>
        /// Multiply every element in this vector by the coprrisponding element in an other vector
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Vector3 Multiply(this Vector3 source, Vector3 other)
        {
            return new Vector3()
            {
                x = source.x * other.x,
                y = source.y * other.y,
                z = source.z * other.z
            };
        }


        /// <summary>
        /// Multiply every element in this vector by the coprrisponding element in an other vector
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Vector2 Multiply(this Vector2 source, Vector2 other)
        {
            return new Vector2()
            {
                x = source.x * other.x,
                y = source.y * other.y
            };
        }

        /// <summary>
        /// Absolute values of all the elements of this vector
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns>a copy of this Vector with all it's elements made positive</returns>
        public static Vector3 Abs(this Vector3 source)
        {
            return new Vector3()
            {
                x = Mathf.Abs(source.x),
                y = Mathf.Abs(source.y),
                z = Mathf.Abs(source.z)
            };
        }
        #endregion

        #region "String formatting"
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

        //turns "TheCatSatOnTheMat" into "The Cat Sat On The Mat"
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
        #endregion


        #region "Array Methods"

        /// <summary>
        /// Find the index of the first instance of an item within the array
        /// </summary>
        /// <typeparam name="IComparable">type of items in the array, which must be comparable</typeparam>
        /// <param name="subject">array of items to be searched</param>
        /// <param name="indexee">item to search for</param>
        /// <returns>-1 if no match is found, or the index at which indexee is located in subject</returns>
        public static int IndexOf<IComparable>(this IComparable[] subject, IComparable indexee)
        {
            if (subject == null)
                return -1;

            if (subject.Length == 0)
                return -1;

            for(int index=0;index < subject.Length;index++)
            {
                if (subject[index].Equals(indexee))
                    return index;
            }

            return -1;
        }

        /// <summary>
        /// Is index a valid position with the array?
        /// </summary>
        /// <param name="subject">the array being tested</param>
        /// <param name="index">possible index within the array</param>
        /// <returns>true if index is a valid position in the array</returns>
        public static bool IsValidIndex(this Array subject, int index)
        {
            if (index < subject.GetLowerBound(0))
                return false;

            int length = subject.Length;

            if (length == 0)
                return false;

            return index <= subject.GetUpperBound(0);
        }

        /// <summary>
        /// Try and get the value of an array at a given index
        /// </summary>
        /// <typeparam name="T">type of values in the array</typeparam>
        /// <param name="array">array to be inspected</param>
        /// <param name="index">index within the array to retrive from</param>
        /// <param name="result">variable to be overwritten with the value</param>
        /// <returns>true on success, false if the index was invalid</returns>
        public static bool TryGet<T>(this T[] array, int index, out T result)
        {
            if (array.IsValidIndex(index))
            {
                result = array[index];

                return true;
            } else
            {
                result = default(T);

                return false;
            }
        }

        /// <summary>
        /// Value of an array at a given index or the default of the type if out of bounds
        /// </summary>
        /// <typeparam name="T">type of values in the array</typeparam>
        /// <param name="array">array to be inspected</param>
        /// <param name="index">index within the array to ge the value of</param>
        /// <returns>content of the array at the given index or the default value of it's type</returns>
        public static T GetOrDefault<T>(this T[] array, int index)
        {
            if (array.IsValidIndex(index))
            {
                return array[index];
            } else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Fill a single dimensional array with a given value
        /// </summary>
        /// <typeparam name="T">type of values in the array</typeparam>
        /// <param name="array">array to be filled</param>
        /// <param name="value">value to fill the array with</param>
        public static void Fill<T>(this T[] array, T value)
        {
            for(int index = array.GetLowerBound(0); index <= array.GetUpperBound(0); index++)
            {
                array[index] = value;
            }
        }

        /// <summary>
        /// Fill a two dimensional array with a given value
        /// </summary>
        /// <typeparam name="T">type of values in the array</typeparam>
        /// <param name="array">array to be filled</param>
        /// <param name="value">value to fill the array with</param>
        public static void Fill<T>(this T[,] array, T value)
        {
            for (int firstIndex = array.GetLowerBound(0); firstIndex <= array.GetUpperBound(0); firstIndex++)
            {
                for (int secondIndex = array.GetLowerBound(1); secondIndex <= array.GetUpperBound(1); secondIndex++)
                {
                    array[firstIndex, secondIndex] = value;
                }
            }
        }

        /// <summary>
        /// Fill a single dimensional array of integers with the indexes of it's elements
        /// </summary>
        /// <param name="array">array to be filled</param>
        public static void IndexFill(this int[] array)
        {
            for (int index = array.GetLowerBound(0); index <= array.GetUpperBound(0); index++)
            {
                array[index] = index;
            }
        }

        #endregion

        #region "Colors"

        /// <summary>
        /// Get the hex representation of this color
        /// </summary>
        /// <param name="source"></param>
        /// <returns>a string of hex digits representing the current color (compatible with HTML & CSS)</returns>
        public static string ToHex(this Color source)
        {
            string result;
            byte[] bytes = new byte[4];
            bytes[0] = (byte)System.Math.Floor(source.a * 255f);
            bytes[1] = (byte)System.Math.Floor(source.r * 255f);
            bytes[2] = (byte)System.Math.Floor(source.g * 255f);
            bytes[3] = (byte)System.Math.Floor(source.b * 255f);

            if (bytes[0] == 255)
            {
                result = string.Format("#{0:X2}{1:X2}{2:X2}", bytes[1], bytes[2], bytes[3]);
            }
            else
            {
                result = string.Format("#{0:X2}{1:X2}{2:X2}{2:X2}", bytes[0], bytes[1], bytes[2], bytes[3]);
            }

            return result;
        }

        /// <summary>
        /// Parse a HTML hex representation of color
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="hex">a string of hex digits (optionally preceeded by a hash symbol) with channels in the order ARGB (alpha being optional) all channels being either one or two hex digists</param>
        public static void ParseHex(this Color subject, string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return;
            }

            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            if (hex.Length > 8)
            {
                hex = hex.Substring(0, 8);
            }

            //expand short 3 digit code (no alpha)
            //abc => aabbcc
            if (hex.Length == 3)
            {
                hex = hex.Substring(0, 1) + hex.Substring(0, 1) + hex.Substring(2, 1) + hex.Substring(2, 1) + hex.Substring(3, 1) + hex.Substring(3, 1);
            }

            //expand short 4 digit code (including alpha)
            //abcd = > aabbccdd
            if (hex.Length == 4)
            {
                hex = hex.Substring(0, 1) + hex.Substring(0, 1) + hex.Substring(2, 1) + hex.Substring(2, 1) + hex.Substring(3, 1) + hex.Substring(3, 1) + hex.Substring(4, 1) + hex.Substring(4, 1);
            }

            //expand codes with alpha cannel ommited
            if (hex.Length == 6)
            {
                hex = "FF" + hex;
            }

            subject.a = (float)System.Convert.ToByte(hex.Substring(0, 2), 16) / 255f;
            subject.r = (float)System.Convert.ToByte(hex.Substring(2, 2), 16) / 255f;
            subject.g = (float)System.Convert.ToByte(hex.Substring(4, 2), 16) / 255f;
            subject.b = (float)System.Convert.ToByte(hex.Substring(6, 2), 16) / 255f;
        } //end ParseHex
        #endregion
    }
}