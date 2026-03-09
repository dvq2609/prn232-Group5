# Plan: CI Pipeline (Tích hợp liên tục cho GitHub)
Created: 2026-03-09 10:48
Status: 🟡 In Progress

## Overview
Dự án cần tự động hóa việc kiểm tra mã nguồn (build) cho cả Backend (.NET) và Frontend (npm) mỗi khi có Pull Request hoặc Push code lên nhánh `main`. Nhờ đó phát hiện sớm lỗi biên dịch.

## Tech Stack
- Nền tảng CI: GitHub Actions
- Backend: .NET
- Frontend: Node.js (npm)

## Phases

| Phase | Name | Status | Progress |
|-------|------|--------|----------|
| 01 | Setup GitHub Workflow | ✅ Complete | 100% |
| 02 | Backend Build Step | ✅ Complete | 100% |
| 03 | Frontend Build Step | ✅ Complete | 100% |

## Quick Commands
- Start Phase 1: `/code phase-01`
- Check progress: `/next`
- Save context: `/save-brain`
