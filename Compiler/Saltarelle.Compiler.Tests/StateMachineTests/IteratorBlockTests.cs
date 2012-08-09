﻿using NUnit.Framework;

namespace Saltarelle.Compiler.Tests.StateMachineTests
{
	[TestFixture]
	public class IteratorBlockTests : StateMachineRewriterTestBase {
		[Test]
		public void IteratorBlockWithoutYieldBreak() {
			AssertCorrect(@"
{
	a;
	yield return b;
	c;
	yield return d;
lbl1:
	e;
}", 
@"{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				setCurrent(b);
				$tmp1 = 1;
				return true;
			}
			case 1: {
				$tmp1 = -1;
				c;
				setCurrent(d);
				$tmp1 = 2;
				return true;
			}
			case 2: {
				$tmp1 = -1;
				e;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void IteratorBlockWithYieldBreak() {
			AssertCorrect(@"
{
	a;
	yield return b;
	c;
	yield break;
}", 
@"{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				setCurrent(b);
				$tmp1 = 1;
				return true;
			}
			case 1: {
				$tmp1 = -1;
				c;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void IteratorBlockWithYieldBreakWithStuffFollowing() {
			AssertCorrect(@"
{
	a;
	yield return b;
	c;
	yield break;
	d;
}", 
@"{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				setCurrent(b);
				$tmp1 = 1;
				return true;
			}
			case 1: {
				$tmp1 = -1;
				c;
				break $loop1;
				d;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldInTryWithFinally1() {
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield return 1;
		c;
	}
	finally {
		d;
	}
	e;
}",
@"$finally1 = function() {
	d;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				b;
				setCurrent(1);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				c;
				$tmp1 = 3;
				continue $loop1;
			}
			case 3: {
				$tmp1 = 2;
				;
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				e;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldInTryWithFinally2() {
			AssertCorrect(
@"{
	a;
	{
		try {
			b;
			yield return 1;
			c;
		}
		finally {
			d;
		}
	}
	e;
}",
@"$finally1 = function() {
	d;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				b;
				setCurrent(1);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				c;
				$tmp1 = 3;
				continue $loop1;
			}
			case 3: {
				$tmp1 = 2;
				;
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				e;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldInTryWithFinally3() {
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield return 1;
		c;
	}
	finally {
		d;
	}
}",
@"$finally1 = function() {
	d;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 1;
				b;
				setCurrent(1);
				$tmp1 = 3;
				return true;
			}
			case 3: {
				$tmp1 = 1;
				c;
				$tmp1 = 2;
				continue $loop1;
			}
			case 2: {
				$tmp1 = 1;
				;
				$tmp1 = -1;
				$finally1();
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldInTryWithFinally4() {
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield return 1;
		c;
	}
	finally {
		d;
	}
	lbl2: e;
}",
@"$finally1 = function() {
	d;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				b;
				setCurrent(1);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				c;
				$tmp1 = 3;
				continue $loop1;
			}
			case 3: {
				$tmp1 = 2;
				;
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				e;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void IteratorFinallyCanContainLabelAndGoto() {
			AssertCorrect(
@"{
	a;
	try {
		yield return 0;
	}
	finally {
		b;
		lbl1:
		c;
		goto lbl1;
	}
	d;
}",
@"$finally1 = function() {
	$tmp1 = 1;
	$loop2:
	for (;;) {
		switch ($tmp1) {
			case 1: {
				$tmp1 = -1;
				b;
				$tmp1 = 2;
				continue $loop2;
			}
			case 2: {
				$tmp1 = -1;
				c;
				$tmp1 = 2;
				continue $loop2;
			}
		}
	}
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 4;
				setCurrent(0);
				$tmp1 = 5;
				return true;
			}
			case 5: {
				$tmp1 = 4;
				;
				$tmp1 = -1;
				$finally1();
				$tmp1 = 3;
				continue $loop1;
			}
			case 3: {
				$tmp1 = -1;
				d;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldBreakExecutesFinallyHandlers1() {
			AssertCorrect(
@"{
	a;
	try {
		yield return 0;
		b;
		yield break;
		c;
	}
	finally {
		d;
	}
	e;
}",
@"$finally1 = function() {
	d;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				setCurrent(0);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				b;
				$tmp1 = -1;
				$finally1();
				break $loop1;
				c;
				$tmp1 = 3;
				continue $loop1;
			}
			case 3: {
				$tmp1 = 2;
				;
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				e;
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldBreakExecutesFinallyHandlersNested() {
			// Finally blocks are executed in the wrong order. The innermost statement contains no labels or yield, so it is preserved.
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield return 1;
		c;
		try {
			d;
			yield return 2;
			e;
			try {
				f;
				yield break;
				g;
			}
			finally {
				h;
			}
			i;
			yield return 4;
			j;
		}
		finally {
			k;
		}
		l;
		yield return 5;
		m;
	}
	finally {
		n;
	}
	o;
}",
@"$finally1 = function() {
	n;
};
$finally2 = function() {
	k;
};
$finally3 = function() {
	h;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				b;
				setCurrent(1);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				c;
				$tmp1 = 6;
				d;
				setCurrent(2);
				$tmp1 = 8;
				return true;
			}
			case 3: {
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				o;
				break $loop1;
			}
			case 8: {
				$tmp1 = 6;
				e;
				$tmp1 = 10;
				f;
				$tmp1 = 6;
				$finally3();
				$tmp1 = 2;
				$finally2();
				$tmp1 = -1;
				$finally1();
				break $loop1;
				g;
				$tmp1 = 11;
				continue $loop1;
			}
			case 7: {
				$tmp1 = 2;
				$finally2();
				$tmp1 = 5;
				continue $loop1;
			}
			case 5: {
				$tmp1 = 2;
				l;
				setCurrent(5);
				$tmp1 = 12;
				return true;
			}
			case 11: {
				$tmp1 = 6;
				$finally3();
				$tmp1 = 9;
				continue $loop1;
			}
			case 9: {
				$tmp1 = 6;
				i;
				setCurrent(4);
				$tmp1 = 13;
				return true;
			}
			case 12: {
				$tmp1 = 2;
				m;
				$tmp1 = 3;
				continue $loop1;
			}
			case 13: {
				$tmp1 = 6;
				j;
				$tmp1 = 7;
				continue $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void GotoOutOfTryBlockExecutesFinallyHandlers() {
			// Finally blocks are executed in the wrong order. The innermost statement contains no labels or yield, so it is preserved.
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield return 1;
		c;
		try {
			d;
			yield return 2;
			e;
			try {
				f;
				goto lbl1;
				g;
			}
			finally {
				h;
			}
			i;
			yield return 4;
			j;
		}
		finally {
			k;
		}
		l;
		yield return 5;
lbl1:
		m;
	}
	finally {
		n;
	}
	o;
}",
@"$finally1 = function() {
	n;
};
$finally2 = function() {
	k;
};
$finally3 = function() {
	h;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				b;
				setCurrent(1);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				c;
				$tmp1 = 6;
				d;
				setCurrent(2);
				$tmp1 = 8;
				return true;
			}
			case 3: {
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				o;
				break $loop1;
			}
			case 8: {
				$tmp1 = 6;
				e;
				$tmp1 = 10;
				f;
				$tmp1 = 6;
				$finally3();
				$tmp1 = 2;
				$finally2();
				$tmp1 = 12;
				continue $loop1;
				g;
				$tmp1 = 11;
				continue $loop1;
			}
			case 7: {
				$tmp1 = 2;
				$finally2();
				$tmp1 = 5;
				continue $loop1;
			}
			case 5: {
				$tmp1 = 2;
				l;
				setCurrent(5);
				$tmp1 = 12;
				return true;
			}
			case 11: {
				$tmp1 = 6;
				$finally3();
				$tmp1 = 9;
				continue $loop1;
			}
			case 9: {
				$tmp1 = 6;
				i;
				setCurrent(4);
				$tmp1 = 13;
				return true;
			}
			case 12: {
				$tmp1 = 2;
				m;
				$tmp1 = 3;
				continue $loop1;
			}
			case 13: {
				$tmp1 = 6;
				j;
				$tmp1 = 7;
				continue $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void CanYieldBreakFromTryWithCatch() {
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield break;
	}
	catch (e) {
		c;
		yield break;
	}
}",
@"{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				try {
					b;
					break $loop1;
				}
				catch (e) {
					c;
					break $loop1;
				}
				break $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}

		[Test]
		public void YieldInTryWithFinallyNested() {
			AssertCorrect(
@"{
	a;
	try {
		b;
		yield return 1;
		c;
		try {
			d;
			yield return 2;
			e;
			try {
				f;
				yield return 3;
				g;
			}
			finally {
				h;
			}
			i;
			yield return 4;
			j;
		}
		finally {
			k;
		}
		l;
		yield return 5;
		m;
	}
	finally {
		n;
	}
	o;
}",
@"$finally1 = function() {
	n;
};
$finally2 = function() {
	k;
};
$finally3 = function() {
	h;
};
{
	var $tmp1 = 0;
	$loop1:
	for (;;) {
		switch ($tmp1) {
			case 0: {
				$tmp1 = -1;
				a;
				$tmp1 = 2;
				b;
				setCurrent(1);
				$tmp1 = 4;
				return true;
			}
			case 4: {
				$tmp1 = 2;
				c;
				$tmp1 = 6;
				d;
				setCurrent(2);
				$tmp1 = 8;
				return true;
			}
			case 3: {
				$tmp1 = 2;
				;
				$tmp1 = -1;
				$finally1();
				$tmp1 = 1;
				continue $loop1;
			}
			case 1: {
				$tmp1 = -1;
				o;
				break $loop1;
			}
			case 8: {
				$tmp1 = 6;
				e;
				$tmp1 = 10;
				f;
				setCurrent(3);
				$tmp1 = 12;
				return true;
			}
			case 7: {
				$tmp1 = 6;
				;
				$tmp1 = 2;
				$finally2();
				$tmp1 = 5;
				continue $loop1;
			}
			case 5: {
				$tmp1 = 2;
				l;
				setCurrent(5);
				$tmp1 = 13;
				return true;
			}
			case 12: {
				$tmp1 = 10;
				g;
				$tmp1 = 11;
				continue $loop1;
			}
			case 11: {
				$tmp1 = 10;
				;
				$tmp1 = 6;
				$finally3();
				$tmp1 = 9;
				continue $loop1;
			}
			case 9: {
				$tmp1 = 6;
				i;
				setCurrent(4);
				$tmp1 = 14;
				return true;
			}
			case 13: {
				$tmp1 = 2;
				m;
				$tmp1 = 3;
				continue $loop1;
			}
			case 14: {
				$tmp1 = 6;
				j;
				$tmp1 = 7;
				continue $loop1;
			}
		}
	}
	return false;
}
", isIteratorBlock: true);
		}
	}
}
