using System;
using UnityEngine;

// Token: 0x02000012 RID: 18
public static class BezierCurve
{
	// Token: 0x06000057 RID: 87 RVA: 0x00004509 File Offset: 0x00002709
	public static Vector3 CubicBezier(Vector3 Start, Vector3 _P1, Vector3 _P2, Vector3 end, float _t)
	{
		return (1f - _t) * BezierCurve.QuadraticBezier(Start, _P1, _P2, _t) + _t * BezierCurve.QuadraticBezier(_P1, _P2, end, _t);
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00004538 File Offset: 0x00002738
	public static Vector3 QuadraticBezier(Vector3 start, Vector3 _P1, Vector3 end, float _t)
	{
		return (1f - _t) * BezierCurve.LinearBezier(start, _P1, _t) + _t * BezierCurve.LinearBezier(_P1, end, _t);
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00004561 File Offset: 0x00002761
	public static Vector3 LinearBezier(Vector3 start, Vector3 end, float _t)
	{
		return (1f - _t) * start + _t * end;
	}
}
