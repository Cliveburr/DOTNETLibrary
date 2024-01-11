using Runner.Kernel.Events.Command;
using Runner.Kernel.Events.Read;
using Runner.Kernel.Events.Write;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Kernel.Events
{
    public static class HandlerRegister
    {
        public static List<Type> CommandHandlers { get; private set; }
        public static List<Type> CommandResultHandlers { get; private set; }
        public static List<Type> ReadHandlers { get; private set; }
        public static List<Type> WriteHandlers { get; private set; }

        private static Dictionary<string, Type> _commandResultCache;
        private static Dictionary<string, Type> _commandCache;
        private static Dictionary<string, Type> _readCache;
        private static Dictionary<string, Type> _writeCache;

        static HandlerRegister()
        {
            CommandHandlers = new List<Type>();
            CommandResultHandlers = new List<Type>();
            ReadHandlers = new List<Type>();
            WriteHandlers = new List<Type>();

            _commandCache = new Dictionary<string, Type>();
            _commandResultCache = new Dictionary<string, Type>();
            _readCache = new Dictionary<string, Type>();
            _writeCache = new Dictionary<string, Type>();
        }

        public static Type GetCommandHandler(Type command)
        {
            var cacheKey = command.FullName!;
            if (_commandCache.ContainsKey(cacheKey))
            {
                return _commandCache[cacheKey];
            }
            else
            {
                var commandHandlerType = typeof(ICommandHandler<>);
                var span = CollectionsMarshal.AsSpan(CommandHandlers);
                foreach (var handler in span)
                {
                    var handlerInterface = handler.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && commandHandlerType.IsAssignableFrom(i.GetGenericTypeDefinition()));
                    if (handlerInterface != null)
                    {
                        var tRequest = handlerInterface.GenericTypeArguments[0];
                        if (tRequest.Equals(command))
                        {
                            _commandCache[cacheKey] = handler;
                            return handler;
                        }
                    }
                }
                throw new Exception("CommandHandler not found for: " + command.FullName);
            }
        }

        public static Type GetCommandResultHandler(Type commandResult)
        {
            var cacheKey = commandResult.FullName!;
            if (_commandResultCache.ContainsKey(cacheKey))
            {
                return _commandResultCache[cacheKey];
            }
            else
            {
                var commandResultHandlerType = typeof(ICommandResultHandler<,>);
                var span = CollectionsMarshal.AsSpan(CommandResultHandlers);
                foreach (var handler in span)
                {
                    var handlerInterface = handler.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && commandResultHandlerType.IsAssignableFrom(i.GetGenericTypeDefinition()));
                    if (handlerInterface != null)
                    {
                        var tRequest = handlerInterface.GenericTypeArguments[0];
                        if (tRequest.Equals(commandResult))
                        {
                            _commandResultCache[cacheKey] = handler;
                            return handler;
                        }
                    }
                }
                throw new Exception("CommandResultHandler not found for: " + commandResult.FullName);
            }
        }

        public static Type GetReadHandler(Type read)
        {
            var cacheKey = read.FullName!;
            if (_readCache.ContainsKey(cacheKey))
            {
                return _readCache[cacheKey];
            }
            else
            {
                var readHandlerType = typeof(IReadHandler<,>);
                var span = CollectionsMarshal.AsSpan(ReadHandlers);
                foreach (var handler in span)
                {
                    var handlerInterface = handler.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && readHandlerType.IsAssignableFrom(i.GetGenericTypeDefinition()));
                    if (handlerInterface != null)
                    {
                        var tRequest = handlerInterface.GenericTypeArguments[0];
                        if (tRequest.Equals(read))
                        {
                            _readCache[cacheKey] = handler;
                            return handler;
                        }
                    }
                }
                throw new Exception("ReadHandler not found for: " + read.FullName);
            }
        }

        public static Type GetWriteHandler(Type write)
        {
            var cacheKey = write.FullName!;
            if (_writeCache.ContainsKey(cacheKey))
            {
                return _writeCache[cacheKey];
            }
            else
            {
                var writeHandlerType = typeof(IWriteHandler<>);
                var span = CollectionsMarshal.AsSpan(WriteHandlers);
                foreach (var handler in span)
                {
                    var handlerInterface = handler.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && writeHandlerType.IsAssignableFrom(i.GetGenericTypeDefinition()));
                    if (handlerInterface != null)
                    {
                        var tRequest = handlerInterface.GenericTypeArguments[0];
                        if (tRequest.Equals(write))
                        {
                            _writeCache[cacheKey] = handler;
                            return handler;
                        }
                    }
                }
                throw new Exception("WriteHandler not found for: " + write.FullName);
            }
        }
    }
}
