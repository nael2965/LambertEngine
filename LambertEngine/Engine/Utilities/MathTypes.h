#pragma once
#include "CommonHeaders.h"

namespace lambert::math
{
    constexpr float pi          = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679f;
    constexpr float epsilon     = 1e-5f;
#if defined(_WIN64)
    
    using V2 = DirectX::XMFLOAT2;
    using V3 = DirectX::XMFLOAT3;
    using V4 = DirectX::XMFLOAT4;
    
    using V2A = DirectX::XMFLOAT2A;
    using V3A = DirectX::XMFLOAT3A;
    using V4A = DirectX::XMFLOAT4A;

    using U32V2 = DirectX::XMUINT2;
    using U32V3 = DirectX::XMUINT3;
    using U32V4 = DirectX::XMUINT4;
    
    using S32V2 = DirectX::XMINT2;
    using S32V3 = DirectX::XMINT3;
    using S32V4 = DirectX::XMINT4;

    using M33 = DirectX::XMFLOAT3X3;
    
    using M44 = DirectX::XMFLOAT4X4;
    using M44A = DirectX::XMFLOAT4X4A;
    
    
}
#endif
