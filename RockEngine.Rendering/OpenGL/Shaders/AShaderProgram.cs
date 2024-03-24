using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common.Editor;
using RockEngine.Common.Utils;

namespace RockEngine.Rendering.OpenGL.Shaders
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
            Check.IsEmpty(baseShaders, "Can't create shader program without shaders");

            Name = name;
            _shaders = baseShaders.ToList();
            if(!AllShaders.TryAdd(Name, this))
            {
                throw new Exception($"Same shader with name:{Name} is already contains");
            }

            _path = _shaders[0].GetFilePath();
        }

        public ISetuppable Setup(IRenderingContext context)
        {
            if(!IsSetupped)
            {
                Setup(context, true);
            }
            return this;
        }

        /// <summary>
        /// Привязка шейдера, после привязки до тех пор пока не будет привязан другой шейдер или не отвязан данный (<see cref="Unbind"/>) 
        /// Все Uniform и тп будут отправляться в привязанный шейдер
        /// </summary>
        public override AShaderProgram Bind(IRenderingContext context)
        {

            _prevActiveShader = _activeShader;
            _activeShader = this;
            context.UseProgram(Handle);
            return this;
        }

        // <summary>
        /// Отвязка шейдера, пока что реализовал так,что ставится на предыдущий шейдер, не знаю насколько это полезно, но думаю что пока сойдет
        /// </summary>
        public override AShaderProgram Unbind(IRenderingContext context)
        {

            var handle = IGLObject.EMPTY_HANDLE;
            var current = _activeShader;
            if(_prevActiveShader != null)
            {
                _activeShader = _prevActiveShader;
                handle = _activeShader.Handle;
            }
            _prevActiveShader = current;

            context.UseProgram(handle);

            return this;
        }

        /// <summary>
        /// Получение индекс расположения по названию переменной
        /// Например, в шейдере можно указать строго заданные места переменных, с помощью location(x)... где x - индекс расположения переменной
        /// </summary>
        /// <param name="attribName">название переменной</param>
        /// <returns></returns>

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Handle, attribName);

        public void ResetShader(IRenderingContext context) => Setup(context, false);

        public UniformFieldInfo[] GetUniforms(IRenderingContext context)
        {
            context.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int unifromCount);

            var uniforms = new UniformFieldInfo[unifromCount];

            for(var i = 0; i < unifromCount; i++)
            {

                context
                    .GetActiveUniform(Handle, i, 256, out var size, out var type, out string name) // get the name of this uniform
                    .GetUniformLocation(Handle, name, out int location); // get the location

                uniforms[i] = new UniformFieldInfo
                {
                    Location = location,
                    Name = name,
                    Size = size,
                    Type = type
                };
            }

            return uniforms;
        }

        public List<UniformFieldInfo> GetMaterialUniforms(IRenderingContext context)
        {
            List<UniformFieldInfo> materialData = new List<UniformFieldInfo>();
            var uniforms = GetUniforms(context);
            foreach(var item in uniforms)
            {
                if(item.Name.StartsWith("Material", StringComparison.OrdinalIgnoreCase))
                {
                    materialData.Add(item);
                }
            }

            return materialData;
        }

        /// <summary>
        /// Отправка в шейдер булевую переменную
        /// в GLSL нельзя вроде отправить булевую переменную поэтому перевод в целочисленный тип,
        /// где 1 == true, а 0 == false
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, bool data)
        {
            context.SetShaderData(location, data ? 1 : 0);
            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context,string name, object data)
        {
            var loc = GetUniformLocation(context, name);

            if(data is int di)
            {
                SetShaderData(context, loc, di);
            }
            else if(data is float df)
            {
                SetShaderData(context, loc, df);
            }
            else if(data is Vector2 dv2)
            {
                SetShaderData(context,loc, dv2);
            }
            else if(data is Vector3 dv3)
            {
                SetShaderData(context, loc, dv3);
            }
            else if(data is Vector4 dv4)
            {
                SetShaderData(context, loc, dv4);
            }
            else if(data is Matrix4 m4)
            {
                SetShaderData(context, loc, m4);
            }

            // fill more if needed
            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context, string name, Vector4[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                context.SetShaderData(GetUniformLocation(context, $"{name}[{i}]"), data[i]);
            }

            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context, int location, Color4 data) => SetShaderData(context, location, new Vector4(data.R, data.G, data.B, data.A));

        /// <summary>
        /// Отправка в шейдер плавающее число
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, float data)
        {
            context.SetShaderData(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="z">координата Z</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, float x, float y, float z) => SetShaderData(context, location, new Vector3(x, y, z));

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, int data)
        {
            context.SetShaderData(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, Matrix4 data)
        {
            context.SetShaderData(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="location">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, Vector2 data)
        {
            context.SetShaderData(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="location">Расположение переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, int location, Vector3 data)
        {
            context.SetShaderData(location, data);
            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context, int location, Vector4 data)
        {
            context.SetShaderData(location, data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер булевую переменную
        /// в GLSL нельзя вроде отправить булевую переменную поэтому перевод в целочисленный тип,
        /// где 1 == true, а 0 == false
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, bool data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data ? 1 : 0);
            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context, string name, Color4 data) => SetShaderData(context, name, new Vector4(data.R, data.G, data.B, data.A));

        /// <summary>
        /// Отправка в шейдер плавающее число
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, float data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="x">координата X</param>
        /// <param name="y">координата Y</param>
        /// <param name="z">координата Z</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, float x, float y, float z) => SetShaderData(context, name, new Vector3(x, y, z));

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, int data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, Matrix4 data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер матрицу 4x4
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, ref Matrix4 data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, ref Vector2 data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, ref Vector3 data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context, string name, ref Vector4 data)
        {
            context.SetShaderData (GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер целочисленную переменную 
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, uint data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 2D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, Vector2 data)
        {
            GL.Uniform2(GetUniformLocation(context, name), data);
            return this;
        }

        /// <summary>
        /// Отправка в шейдер 3D вектор
        /// </summary>
        /// <param name="name">Название переменной в шейдере</param>
        /// <param name="data">Отправляемая переменная</param>
        public AShaderProgram SetShaderData(IRenderingContext context, string name, Vector3 data)
        {
            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        public AShaderProgram SetShaderData(IRenderingContext context, string name, Vector4 data)
        {

            context.SetShaderData(GetUniformLocation(context, name), data);
            return this;
        }

        public override AShaderProgram SetLabel(IRenderingContext context)
        {
            string name = $"Shader program ({Handle}), with name: {Name}";
            Logger.AddLog($"Setupped {name}");
            context.ObjectLabel(ObjectLabelIdentifier.Program, Handle, name.Length, name);
            return this;
        }

        /// <summary>
        /// Получение расположение переменной в шейдере,
        /// Если нам уже известно расположение,то возвращаем,
        /// иначе ищем с помощью <see cref="GL"/>
        /// </summary>
        /// <param name="name">Название переменной</param>
        private int GetUniformLocation(IRenderingContext context, string name)
        {
            if(UniformLocations.TryGetValue(name, out int value))
            {
                return value;
            }
            else
            {
                context.GetUniformLocation(Handle, name, out int location);
                UniformLocations.TryAdd(name, location);
                if(location == -1)
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
        private void LinkProgram(IRenderingContext context, int program)
        {
            // We link the program
            context.LinkProgram(program, out int status);

            if(status != (int)All.True)
            {
                context.GetProgramInfoLog(program, out var infoLog);
                Logger.AddError(infoLog);
                throw new Exception($"{infoLog}, FileName - {_path},ERROR: {GL.GetError()}");
            }
        }

        protected void Setup(IRenderingContext context, bool firstLoad)
        {
                if(!firstLoad)
                {
                    // Safe remove shader
                    context.UseProgram(0)
                        .DeleteProgram(Handle);
                }

                if(_shaders.Count == 1)
                {
                    // Create a singleShader to attach to the pipeline
                    var shader = _shaders[0];
                    context.CreateShaderProgram(shader.Type, 1, new string[] { shader.GetShaderText() }, out int handle);
                    Handle = handle;
                }
                else
                {
                    context.CreateProgram(out int handle);
                    Handle = handle;

                    foreach(var item in _shaders)
                    {
                        item.Setup(context, Handle);
                    }

                    LinkProgram(context, Handle);
                    // TODO: Think if it is usefull to disposing them after setupping or not
                    foreach(var item in _shaders)
                    {
                        item.Dispose();
                    }
                }

                SetLabel(context);
                // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
                // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
                // later.

                FillCacheUniforms(context);
            
        }

        private void FillCacheUniforms(IRenderingContext context)
        {
            var uniforms = GetUniforms(context);

            // Next, allocate the dictionary to hold the locations.
            UniformLocations.Clear();
            // Loop over all the uniforms,
            for(var i = 0; i < uniforms.Length; i++)
            {
                var uniform = uniforms[i];
                // and then add it to the dictionary.
                UniformLocations.Add(uniform.Name, uniform.Location);
            }
        }

        protected override void Dispose(bool disposing)
        {
            IRenderingContext.Update(context =>
            {
                if(_disposed)
                {
                    return;
                }
                if(disposing)
                {
                    // Освободите управляемые ресурсы здесь
                }

                if(!IsSetupped)
                {
                    return;
                }

                context.GetObjectLabel(ObjectLabelIdentifier.Program, Handle, 64, out int length, out string name);
                if(name.Length == 0)
                {
                    name = $"Shader program: ({Handle})";
                }
                Logger.AddLog($"Disposing {name}");
                foreach(var item in _shaders)
                {
                    item.Dispose();
                }
                context.DeleteProgram(Handle);

                Handle = IGLObject.EMPTY_HANDLE;
                _disposed = true;
            });

        }

        public override bool IsBinded(IRenderingContext context)
        {
            context.GetInteger(GetPName.CurrentProgram, out int value);
            return value == Handle;
        }

        ~AShaderProgram()
        {
            Dispose();
        }
    }
}
