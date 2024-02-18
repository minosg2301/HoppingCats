using Doozy.Engine;

namespace moonNest
{
    public class EventMessage : Message
    {
        public string content;

        public EventMessage(string content)
        {
            this.content = content;
        }

        public static void SendEvent(string content) => Send(new EventMessage(content));
        public static void SendEvent(string messageName, string content) => Send(messageName, new EventMessage(content));
    }

    public class ProgressMessage : Message
    {
        public string content;
        public float progress;

        public ProgressMessage(float progress, string content)
        {
            this.progress = progress;
            this.content = content;
        }

        public static void SendEvent(float progress, string content) => Send(new ProgressMessage(progress, content));
        public static void SendEvent(string messageName, float progress, string content) => Send(messageName, new ProgressMessage(progress, content));
    }
}