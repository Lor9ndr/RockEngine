using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Editor;
using RockEngine.Utils;

namespace RockEngine.OpenGL.Shaders
{
    public abstract class AShaderProgram : AGLObject, ISetuppable
    {
        /// <summary>
        /// Возможные значения, которые нужно заполнить в шейдере
        /// </summary>
        private readonly Dictionary<string, int> UniformLocations = new Dictionary<string, int>();

        /// <summary>
        /// Cache of the all available shader programs
        /// </summary>
        public static readonly Dictionary<string, AShaderProgram> AllShaders = new Dictionary<string, AShaderProgram>();

        public Guid ID { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Path of the first shader
        /// </summary>
        private readonly string _path;

        [DiableUI]
        public Dictionary<int, IGLObject> BoundUniformBuffers = new Dictionary<int, IGLObject>();

        public static AShaderProgram? ActiveShader => _activeShader;

        /// <summary>
        /// Shaders with different types linked to that shaderProgram
        /// </summary>
        private readonly List<BaseShaderType> _shaders;

        public bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        private static AShaderProgram? _activeShader;
        private static AShaderProgram? _prevActiveShader;

        protected AShaderProgram(string name, params BaseShaderType[] baseShaders)
        {
            Check.IsEmpty(baseShaders, "Can't create shader program withoud shaders");
            
            Name = name;
            _shaders = baseShaders.ToList();
            if(!AllShaders.TryAdd(Name, this))
            {
                throw new Exception($"Same shader with name:{this.Name} is already contains");
            }

            _path = _shaders[0].GetFilePath();
            Setup();
        }

        public ISetuppable Setup()
        {
            if(!IsSetupped)
            {
                Setup(true);
            }
            return this;
        }

        /// <summary>
        /// Привязка шейдера, после привязки до тех пор пока не будет привязан другой шейдер или не отвязан данный (<see cref="Unbind"/>) 
        /// Все Uniform и тп будут отправляться в привязанный шейдер
        /// </summary>
        public override AShaderProgram Bind()
        {
            _prevActiveShader = _activeShader;
            _activeShader = this;
            GL.UseProgram(Handle);
            return this;
        }

        // <summary>
        /// Отвязка шейдера, пока что реализовал так,что ставится на предыдущий шейдер, не знаю насколько это полезно, но думаю что пока сойдет
        /// </summary>
        public override AShaderProgram Unbind()
        {
            var handle = IGLObject.EMPTY_HANDLE;
            var current = _activeShader;
            if(_prevActiveShader != null)
            {
                _activeShader = _prevActiveShader;
                handle = _activeShader.Handle;
            }
            _prevActiveShader = current;

            GL.UseProgram(handle);
            return this;
        }

        /// <summary>
        /// Получение индекс расположения по названию переменной
        /// Например, в шейдере можно указать строго заданные места переменных, с помощью location(x)... где x - индекс расположения переменной
        /// </summary>
        /// <param name="attribName">название переменной</param>
        /// <returns></returns>

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Handle, attribName);

        public void ResetShader() => Setup(false);

        public UniformFieldInfo[] GetUniforms()
        {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int unifromCount);

            var uniforms = new UniformFieldInfo[unifromCount];

            for(var i = 0; i < unifromCount; i++)
            {
                // get the name of this uniform,
                GL.GetActiveUniform(Handle, i, 256,out _, out var size, out var type, out string name);

                // get the location,
                var location = GL.GetUniformLocation(Handle, name);

                uniforms[i] = new UniformFieldInfo
                {
                    Location = location,
                    Name = name,
                    Size = size,
                    Type = type
                };
                ;
            }

            return uniforms;
        }

        public List<UniformFieldInfo> GetMaterialUniforms()
        {
            List<UniformFieldInfo> materialData = new List<UniformFieldInfo>();
            var uniforms = GetUniforms();
            foreach(var item in uniforms)
            {
                if(item.Name.StartsWith("Material", StringComparison.OrdinalIgnoreCase))
                {
                    materialData.Add(item);
                }
            }

            return materialData;
        }

        public UniformFieldInfo[] GetUniformBufferData(int bufferBindingPoint)
        {
            GL.GetActiveUniformBlock(Handle, bufferBindingPoint, ActiveUniformBlockParameter.UniformBlockActiveUniforms, out int cnt);
            UniformFieldInfo[ ] info = new UniformFieldInfo[cnt];
            for(int i = 0; i < cnt; i++)
            {
                //GL.GetActiveUniform(Handle, i, 
            }
            return info;
        }

        /// <summary>
        /// Отправка в шейдер булевую переменную
        /// в GLSL нельзя вроде отправить булевую переменную поэтому перевод в целочисленный тип,
        /// где 1 == true, а 0 == false
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(int location, bool data)
        {
            GL.Uniform1(location, data ? 1 : 0);
            return this;
        }

        public AShaderProgram SetShaderData(string name, object data)
        {
            var loc = GetUniformLocation(name);
            if(data is int di)
            {
                SetShaderData(loc, di);
            }
            else if(data is float df)
            {
                SetShaderData(loc, df);
            }
            else if (data is Vector2 dv2)
            {
                SetShaderData(loc,dv2);
            }
            else if (data is Vector3 dv3)
            {
                SetShaderData(loc, dv3);
            }
            else if (data is Vector4 dv4)
            {
                SetShaderData(loc, dv4);
            }
            else if(data is Matrix4 m4)
            {
                SetShaderData(loc, m4);
            }

            // fill more if needed
            return this;
        }

        public AShaderProgram SetShaderData(string name, Vector4[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                GL.Uniform4(GetUniformLocation($"{name}[{i}]"), data[i]);
            }

            return this;
        }

        public AShaderProgram SetShaderData(int location, Color4 data) => SetShaderData(location, new Vector4(data.R, data.G, data.B, data.A));

        /// <summary>
        /// Отправка в шейдер плавающее число
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(int location, float data)
        {
            GL.Uniform1(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="z">координата Z</param>
        public AShaderProgram SetShaderData(int location, float x, float y, float z) => SetShaderData(location, new Vector3(x, y, z));

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(int location, int data)
        {
            GL.Uniform1(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(int location, Matrix4 data)
        {
            GL.UniformMatrix4(location, false, ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="location">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(int location, Vector2 data)
        {
            GL.Uniform2(location, ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(int location, Vector3 data)
        {
            GL.Uniform3(location, data);
            return this;
        }

        public AShaderProgram SetShaderData(int location, Vector4 data)
        {
            GL.Uniform4(location, ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер булевую переменную
        /// в GLSL нельзя вроде отправить булевую переменную поэтому перевод в целочисленный тип,
        /// где 1 == true, а 0 == false
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, bool data)
        {
            GL.Uniform1(GetUniformLocation(name), data ? 1 : 0);
            return this;
        }

        public AShaderProgram SetShaderData(string name, Color4 data) => SetShaderData(name, new Vector4(data.R, data.G, data.B, data.A));

        /// <summary>
        /// Отправка в шейдер плавающее число
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, float data)
        {
            GL.Uniform1(GetUniformLocation(name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="z">координата Z</param>
        public AShaderProgram SetShaderData(string name, float x, float y, float z) => SetShaderData(name, new Vector3(x, y, z));

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, int data)
        {
            GL.Uniform1(GetUniformLocation(name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, Matrix4 data, bool transpose = false)
        {
            GL.UniformMatrix4(GetUniformLocation(name), transpose, ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, ref Matrix4 data, bool transpose = false)
        {
            GL.UniformMatrix4(GetUniformLocation(name), transpose, ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, ref Vector2 data)
        {
            GL.Uniform2(GetUniformLocation(name), ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, ref Vector3 data)
        {
            GL.Uniform3(GetUniformLocation(name), ref data);
            return this;
        }

        public AShaderProgram SetShaderData(string name, ref Vector4 data)
        {
            GL.Uniform4(GetUniformLocation(name), ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, uint data)
        {
            GL.Uniform1(GetUniformLocation(name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, Vector2 data)
        {
            GL.Uniform2(GetUniformLocation(name), ref data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(string name, Vector3 data)
        {
            GL.Uniform3(GetUniformLocation(name), data);
            return this;
        }

        public AShaderProgram SetShaderData(string name, Vector4 data)
        {
            GL.Uniform4(GetUniformLocation(name), data);
            return this;
        }

        public override AShaderProgram SetLabel()
        {
            string name = $"Shader program ({Handle}), with name: {Name}";
            Logger.AddLog($"Setupped {name}");
            GL.ObjectLabel(ObjectLabelIdentifier.Program, Handle, name.Length, name);
            return this;
        }

        /// <summary>
        /// Получение расположение переменной в шейдере,
        /// Если нам уже известно расположение,то возвращаем,
        /// иначе ищем с помощью <see cref="GL"/>
        /// </summary>
        /// <param name="name">Название переменной</param>
        private int GetUniformLocation(string name)
        {
            if (UniformLocations.TryGetValue(name, out int value))
            {
                return value;
            }
            else
            {
                var location = GL.GetUniformLocation(Handle, name);
                UniformLocations.TryAdd(name, location);
                if (location == -1)
                {
                    var error = $"{name} Was not setted in {_path}";
                    Logger.AddWarn(error);
                }
                return location;
            }
        }

        /// <summary>
        /// Присоединение разных шейдеров в одну программу
        /// </summary>
        /// <param name="program">индекс программы</param>
        /// <exception cref="Exception">Выдает ошибку, если при соединении вышла ошибка, например, когда шейдеры имеют разные in - out переменные </exception>
        private void LinkProgram(int program)
        {
            // We link the program
            GL.LinkProgram(program);

            // Check for linking errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetProgramInfoLog(program);
                Logger.AddError(infoLog);
                throw new Exception($"{infoLog}, FileName - {_path},ERROR: {GL.GetError()}");
            }
        }

        protected void Setup(bool firstLoad)
        {
            if (!firstLoad)
            {
                // Safe remove shader
                GL.UseProgram(0);
                GL.DeleteProgram(Handle);
            }

            if (_shaders.Count == 1)
            {
                // Create a singleShader to attach to the pipeline
                var shader = _shaders[0];
                Handle = GL.CreateShaderProgram(shader.Type, 1, new string[] { shader.GetShaderText() });
            }
            else
            {
                Handle = GL.CreateProgram();

                foreach (var item in _shaders)
                {
                    item.Setup(Handle);
                }

                LinkProgram(Handle);
                // TODO: Think if it is usefull to disposing them after setupping or not
                foreach (var item in _shaders)
                {
                    item.Dispose();
                }
            }

            SetLabel();
            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            FillCacheUniforms();
        }

        private void FillCacheUniforms()
        {
            var uniforms = GetUniforms();

            // Next, allocate the dictionary to hold the locations.
            UniformLocations.Clear();
            // Loop over all the uniforms,
            for (var i = 0; i < uniforms.Length; i++)
            {
                var uniform = uniforms[i];
                // and then add it to the dictionary.
                UniformLocations.Add(uniform.Name, uniform.Location);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // Освободите управляемые ресурсы здесь
            }

            if (!IsSetupped)
            {
                return;
            }
            GL.GetObjectLabel(ObjectLabelIdentifier.Program, Handle, 64, out int length, out string name);
            if (name.Length == 0)
            {
                name = $"Shader program: ({Handle})";
            }
            Logger.AddLog($"Disposing {name}");
            foreach (var item in _shaders)
            {
                item.Dispose();
            }
            GL.DeleteProgram(Handle);
            Handle = IGLObject.EMPTY_HANDLE;
            _disposed = true;
        }

        public override bool IsBinded() => GL.GetInteger(GetPName.CurrentProgram) == Handle;

        ~AShaderProgram()
        {
            Dispose();
        }
    }
}
