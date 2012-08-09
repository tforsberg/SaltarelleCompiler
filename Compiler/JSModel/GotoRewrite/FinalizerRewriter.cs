using System;
using System.Collections.Generic;
using System.Linq;
using Saltarelle.Compiler.JSModel.Expressions;
using Saltarelle.Compiler.JSModel.Statements;

namespace Saltarelle.Compiler.JSModel.GotoRewrite
{
	internal class FinalizerRewriter : RewriterVisitorBase<object>, IGotoStateStatementVisitor<JsStatement, object> {
		private string _stateVariableName;
		private Dictionary<string, State> _labelStates = new Dictionary<string, State>();

		public FinalizerRewriter(string stateVariableName, Dictionary<string, State> labelStates) {
			_stateVariableName = stateVariableName;
			_labelStates = labelStates;
		}

		public JsBlockStatement Process(JsBlockStatement statement) {
			return (JsBlockStatement)VisitStatement(statement, null);
		}

		public override JsStatement VisitGotoStatement(JsGotoStatement statement, object data) {
			throw new InvalidOperationException("Shouldn't happen");
		}

		public override JsStatement VisitFunctionStatement(JsFunctionStatement statement, object data) {
			return statement;
		}

		public override JsExpression VisitFunctionDefinitionExpression(JsFunctionDefinitionExpression expression, object data) {
			return expression;
		}

		public JsStatement VisitGotoStateStatement(JsGotoStateStatement statement, object data) {
			var result = new List<JsStatement>();
			State targetState;
			if (statement.TargetState == null) {
				if (!_labelStates.TryGetValue(statement.TargetLabel, out targetState))
					throw new InvalidOperationException("The label " + statement.TargetLabel + " does not exist.");
			}
			else
				targetState = statement.TargetState.Value;

			var remaining = statement.CurrentState.FinallyStack;
			for (int i = 0, n = remaining.Count() - targetState.FinallyStack.Count(); i < n; i++) {
				var current = remaining.Peek();
				remaining = remaining.Pop();
				result.Add(new JsExpressionStatement(JsExpression.Assign(JsExpression.Identifier(_stateVariableName), JsExpression.Number(remaining.IsEmpty ? -1 : remaining.Peek().Item1))));
				result.Add(new JsExpressionStatement(JsExpression.Invocation(JsExpression.Identifier(current.Item2))));
			}

			if (targetState.StateValue == -1) {
				result.Add(new JsBreakStatement(targetState.LoopLabelName));
			}
			else {
				SingleStateMachineRewriter.SetNextState(result, _stateVariableName, targetState.StateValue);
				result.Add(new JsContinueStatement(targetState.LoopLabelName));
			}
			return new JsBlockStatement(result, mergeWithParent: true);
		}
	}
}