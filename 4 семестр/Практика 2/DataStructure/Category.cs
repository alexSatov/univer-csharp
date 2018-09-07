using System;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        private readonly string productName;
        private readonly MessageType messageType;
        private readonly MessageTopic messageTopic;

        public Category(string name, MessageType type, MessageTopic topic)
        {
            productName = name;
            messageType = type;
            messageTopic = topic;
        }

        public int CompareTo(object obj)
        {
            var other = (Category) obj;

            if (productName != other.productName)
                return string.Compare(productName, other.productName, StringComparison.CurrentCulture);

            if (messageType != other.messageType)
                return messageType > other.messageType ? 1 : -1;

            if (messageTopic != other.messageTopic)
                return messageTopic > other.messageTopic ? 1 : -1;

            return 0;
        }

        public static bool operator ==(Category first, Category second)
        {
            return !ReferenceEquals(first, null) && first.Equals(second);
        }

        public static bool operator !=(Category first, Category second)
        {
            return ReferenceEquals(first, null) || !first.Equals(second);
        }

        public static bool operator >(Category first, Category second)
        {
            return first.CompareTo(second) == 1;
        }

        public static bool operator >=(Category first, Category second)
        {
            return first > second || first == second;
        }

        public static bool operator <(Category first, Category second)
        {
            return first.CompareTo(second) == -1;
        }

        public static bool operator <=(Category first, Category second)
        {
            return first < second || first == second;
        }

        public override string ToString()
        {
            return $"{productName}.{messageType}.{messageTopic}";
        }

        public override bool Equals(object obj)
        {
            var other = obj as Category;

            if (ReferenceEquals(other, null))
                return false;

            return productName == other.productName
                   && messageType == other.messageType
                   && messageTopic == other.messageTopic;
        }

        public override int GetHashCode()
        {
            return 1001*productName.GetHashCode() 
                + 101*messageType.GetHashCode() 
                + 11*messageTopic.GetHashCode();
        }
    }
}
