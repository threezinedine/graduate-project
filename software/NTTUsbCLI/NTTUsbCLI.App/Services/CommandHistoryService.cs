using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTUsbCLI.App.Services
{
	public class CommandHistoryService : ICommandHistoryService
	{
		private List<string> m_strCommands = new List<string>();
		private int m_numCurrentCommandIndex = 0;
		public void AppendCommand(string strCommand)
		{
			m_strCommands.Add(strCommand);
			m_numCurrentCommandIndex = m_strCommands.Count;
		}

		public bool Empty()
		{
			return m_strCommands.Count == 0;
		}

		public string GetCurrentCommand()
		{
			return m_strCommands[m_numCurrentCommandIndex].ToString();
		}

		public void ToNextCommand()
		{
			if (m_numCurrentCommandIndex < m_strCommands.Count - 1) 
			{
				m_numCurrentCommandIndex++;	
			}
		}

		public void ToPreviousCommand()
		{
			if (m_numCurrentCommandIndex >= 1)
			{
				m_numCurrentCommandIndex--;
			}
		}
	}
}
