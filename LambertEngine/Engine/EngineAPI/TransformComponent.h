﻿#pragma once
#include "../Components/ComponentsCommon.h"

namespace lambert::transform
{    
    DEFINE_TYPED_ID(transform_id);
    
    class component final
    {
    public:        
        constexpr explicit component(transform_id id) : _id{ id } {}
        constexpr component() : _id{ id::invalid_id } {}
        constexpr transform_id get_id() const { return _id; }
        constexpr bool is_valid() const { return id::is_valid(_id); }

        math::V4 rotation() const;
        math::V3 position() const;
        math::V3 scale() const;
    private:
        transform_id _id;
    };
}
