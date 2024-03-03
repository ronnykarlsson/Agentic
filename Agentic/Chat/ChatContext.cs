using System.Collections;
using System.Collections.Generic;

namespace Agentic.Chat
{
    public class ChatContext
    {
        public ChatMessageLinkedList Messages { get; private set; } = new ChatMessageLinkedList();

        public void AddMessage(Role role, string content)
        {
            Messages.AddLast(new ChatMessage(role, content));
        }
    }

    public class ChatMessageNode
    {
        public ChatMessage Data { get; set; }
        public ChatMessageNode Next { get; set; }
        public ChatMessageNode Previous { get; set; }

        public ChatMessageNode(ChatMessage data)
        {
            Data = data;
            Next = null;
            Previous = null;
        }
    }

    public class ChatMessageLinkedList : IEnumerable<ChatMessage>
    {
        private ChatMessageNode _head;
        private ChatMessageNode _tail;

        public int Count { get; set; }

        public ChatMessageLinkedList()
        {
            _head = null;
            _tail = null;
        }

        public void AddLast(ChatMessage message)
        {
            ChatMessageNode newNode = new ChatMessageNode(message);
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                newNode.Previous = _tail;
                _tail = newNode;
            }
            Count++;
        }

        public IEnumerator<ChatMessage> GetEnumerator()
        {
            ChatMessageNode current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<ChatMessage> GetReverseEnumerator()
        {
            ChatMessageNode current = _tail;
            while (current != null)
            {
                yield return current.Data;
                current = current.Previous;
            }
        }
    }
}
