﻿#ifndef ROTATION_UTIL
#define ROTATION_UTIL

static const float3 g_positions[4] =
{
    float3(-1, 1, 0),
    float3(1, 1, 0),
    float3(-1,-1, 0),
    float3(1,-1, 0),
};

static const float2 g_texcoords[4] =
{
    float2(0, 1),
    float2(1, 1),
    float2(0, 0),
    float2(1, 0),
};

float4 rotate(float4 v, float angle, float3 axis)
{
    float3 a = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float r = 1.0 - c;
    float3x3 m = float3x3(
        a.x * a.x * r + c,
        a.y * a.x * r + a.z * s,
        a.z * a.x * r - a.y * s,
        a.x * a.y * r - a.z * s,
        a.y * a.y * r + c,
        a.z * a.y * r + a.x * s,
        a.x * a.z * r + a.y * s,
        a.y * a.z * r - a.x * s,
        a.z * a.z * r + c
    );
    return float4(mul(m, v.xyz), 1);
}

float4x4 rotateMtx(float angle, float axis)
{
    float3 a = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float r = 1.0 - c;
    float4x4 m = float4x4(
        a.x * a.x * r + c,
        a.y * a.x * r + a.z * s,
        a.z * a.x * r - a.y * s,
        0, 
        a.x * a.y * r - a.z * s,
        a.y * a.y * r + c,
        a.z * a.y * r + a.x * s,
        0,
        a.x * a.z * r + a.y * s,
        a.y * a.z * r - a.x * s,
        a.z * a.z * r + c,
        0,
        0,
        0,
        0,
        0
    );
    return m;
}

float2 rotateRadians(float2 UV, float2 Center, float Rotation)
{
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;
    return UV;
}

float2 rotateDegrees(float2 UV, float2 Center, float Rotation)
{
    Rotation = Rotation * (3.1415926f / 180.0f);
    return rotateRadians(UV, Center, Rotation);
}

float2 rotate2D(float2 v, float theta)
{
    return float2(v.x * cos(theta) - v.y * sin(theta), v.x * sin(theta) + v.y * cos(theta));
}

float2 lookAt2D(float2 v, float2 currDir, float2 nextDir)
{
    float theta = acos(dot(currDir, nextDir));
    int sn = sign(cross(float3(currDir, 0), float3(nextDir, 0)).z);
    return rotate2D(v, sn * theta);
}

#endif