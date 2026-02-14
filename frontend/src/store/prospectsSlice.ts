import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import api from '../services/api';

export interface Ranking {
  id: string;
  rank: number;
  playerName: string;
  team: string;
  position: string;
  age: number;
  eta: string | null;
  score: number;
  volatility: string;
  consensus: number;
  median: number | null;
  sd: number | null;
  tier: number;
  sourceCount: number;
}

interface ProspectsState {
  rankings: Ranking[];
  isLoading: boolean;
  error: string | null;
}

const initialState: ProspectsState = {
  rankings: [],
  isLoading: false,
  error: null,
};

export const fetchRankings = createAsyncThunk(
  'prospects/fetchRankings',
  async () => {
    const response = await api.get<Ranking[]>('/prospects');
    return response.data;
  }
);

const prospectsSlice = createSlice({
  name: 'prospects',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchRankings.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchRankings.fulfilled, (state, action) => {
        state.isLoading = false;
        state.rankings = action.payload;
      })
      .addCase(fetchRankings.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to fetch rankings';
      });
  },
});

export default prospectsSlice.reducer;
