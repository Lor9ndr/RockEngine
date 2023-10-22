using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Utils;

namespace RockEngine.OpenGL.Shaders
{
    public abstract class AShaderProgram : AGLObject, ISetuppable
    {
        /// <summary>
        /// Возможные значения, которые нужно заполнить в шейдере
        /// </summary>
        private readonly Dictionary<string, int> UniformLocations = new Dictionary<string, int>();

        private readonly DateTime LastUpdateTime;

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

        public static AShaderProgram? ActiveShader
        {
            get => _activeShader;
            set
            {
                _activeShader?.Unbind();
                _activeShader = null;
                value?.Bind();
            }
        }

        /// <summary>
        /// Shaders with different types linked to that shaderProgram
        /// </summary>
        private readonly List<BaseShaderType> _shaders;

        public bool IsSetupped => Handle != IGLObject.EMPTY_HANDLE;

        private static AShaderProgram? _activeShader;
        private static AShaderProgram? _prevActiveShader;

        protected AShaderProgram(string name, params BaseShaderType[] baseShaders)
        {
            Validator.ThrowIfEmpty(baseShaders, "Can't create shader program withoud shaders");
            
            Name = name;
            _shaders = baseShaders.ToList();
            if(!AllShaders.TryAdd(this.Name, this))
            {
                throw new Exception($"Same shader with name:{this.Name} is already contains");
            }

            Setup();
            _path = _shaders[0].GetFilePath();
        }
        public ISetuppable Setup()
        {
            Setup(true);
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
            if (_prevActiveShader != null)
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
        public AShaderProgram SetShaderData(string name, Vector4[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                GL.Uniform4(GetUniformLocation(name + $"[{i}]"), data[i]);
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
                int pos = GL.GetUniformLocation(Handle, name);
                UniformLocations.TryAdd(name, pos);
                if (pos == -1)
                {
                    var error = $"{name} Was not setted in {_path}";
                    Logger.AddWarn(error);
                }
                return pos;
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
            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            UniformLocations.Clear();
            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                UniformLocations.Add(key, location);
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

        ~AShaderProgram()
        {
            Dispose();
        }
    }
}
