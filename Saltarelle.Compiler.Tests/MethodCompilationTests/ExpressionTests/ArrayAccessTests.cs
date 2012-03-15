﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Saltarelle.Compiler.Tests.MethodCompilationTests.ExpressionTests {
	[TestFixture]
	public class ArrayAccessTests : MethodCompilerTestBase {
		[Test]
		public void AccessingAMultiDimensionalArrayIsAnError() {
			var er = new MockErrorReporter(false);
			Compile(new[] { "class Class { public void M(int[,] arr) { var x = arr[0, 0]; } }" }, errorReporter: er);
			Assert.That(er.AllMessages.Any(m => m.StartsWith("Error:") && m.Contains("dimension")));
		}

		[Test]
		public void SimpleArrayAccessWorks() {
			AssertCorrect(
@"void M() {
	var arr = new int[0];
	int i = 0;
	// BEGIN
	int x = arr[i];
	// END
}",
@"	var $x = $arr[$i];
");
		}

		[Test]
		public void ArrayAccessEvaluatesExpressionsInTheCorrectOrder() {
			AssertCorrect(
@"int P { get; set; }
int[] F() { return null; }
void M() {
	int i = 0;
	// BEGIN
	int x = F()[P = i];
	// END
}",
@"	var $tmp1 = this.$F();
	this.set_$P($i);
	var $x = $tmp1[$i];
");
		}
	}
}
