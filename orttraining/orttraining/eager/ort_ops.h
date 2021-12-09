// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma once

#include <core/framework/ort_value.h>
#include <core/eager/ort_kernel_invoker.h>

namespace torch_ort {
namespace eager {

OrtValue reshape_invoke(
  onnxruntime::ORTInvoker& invoker,
  OrtValue& input,
  std::vector<int64_t> shape,
  bool in_place);

OrtValue add(onnxruntime::ORTInvoker& invoker,
             const OrtValue& A,
             const OrtValue& B);

void copy(onnxruntime::ORTInvoker& invoker, 
          const OrtValue& src, OrtValue& dst);

} // namespace eager
} // namespace torch_ort