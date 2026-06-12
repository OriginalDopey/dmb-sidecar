"""
Pytest configuration for MCP bridge unit and integration tests.

Ensures ``src/DmbSidecar.McpBridge`` is on ``sys.path`` so tests can import
``lineup_engine``, ``lineup_config``, and ``app`` without installing the
bridge as a package.
"""

from __future__ import annotations

import sys
from pathlib import Path

# ---------------------------------------------------------------------------
# Import path setup
# ---------------------------------------------------------------------------

BRIDGE_ROOT = Path(__file__).resolve().parents[2] / "src" / "DmbSidecar.McpBridge"
"""Absolute path to the FastAPI MCP bridge source directory."""

if str(BRIDGE_ROOT) not in sys.path:
    sys.path.insert(0, str(BRIDGE_ROOT))
