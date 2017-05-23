using System;

namespace ProManClient.ViewModels {
    public class BaseViewModel {

        public DateTime StartDate {
            get {
                return PoseidonWeb.Helpers.Classic.Sessions.Get<DateTime>( "StartDate" );
            }
            set { }
        }
        public DateTime EndDate {
            get {
                return PoseidonWeb.Helpers.Classic.Sessions.Get<DateTime>( "EndDate" );
            }
            set { }
        }

        protected double CalculateEffort( int cocomoMode, float totalBytes, float BPL ) {
            if ( totalBytes < 0 )
                totalBytes = Math.Abs( totalBytes / 10 );
            switch ( cocomoMode ) {
                case 0:
                    return 2.4 * Math.Pow( totalBytes / BPL / 1024, 1.05 );
                case 1:
                    return 3.0 * Math.Pow( totalBytes / BPL / 1024, 1.12 );
                case 2:
                    return 3.6 * Math.Pow( totalBytes / BPL / 1024, 1.20 );
            }
            return 0;
        }

        protected double CalculateDevTime( int cocomoMode, float totalBytes, float BPL ) {
            var e = CalculateEffort( cocomoMode, totalBytes, BPL );
            switch ( cocomoMode ) {
                case 0:
                    return 2.5 * Math.Pow( e, 0.38 );
                case 1:
                    return 2.5 * Math.Pow( e, 0.35 );
                case 2:
                    return 2.5 * Math.Pow( e, 0.32 );
            }
            return 0;
        }

        protected double CalculatePeopleNeeded( int cocomoMode, float totalBytes, float BPL ) {
            var e = CalculateEffort( cocomoMode, totalBytes, BPL );
            var d = CalculateDevTime( cocomoMode, totalBytes, BPL );
            return e / d;
        }
    }
}