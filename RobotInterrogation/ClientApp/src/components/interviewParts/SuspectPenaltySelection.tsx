import * as React from 'react';
import { renderOptions } from './renderOptions';
import { PositionHeader } from './elements/PositionHeader';
import { InterviewPosition } from '../interviewReducer';

interface IProps {
    options: string[],
    action: (index: number) => void,
}

export const SuspectPenaltySelection: React.FunctionComponent<IProps> = props => {
    return (
        <div>
            <PositionHeader position={InterviewPosition.Suspect} />
            <p>Select one of the following penalties to <strong>use</strong> for this interview.</p>

            {renderOptions(props.options, props.action)}

            <p>The penalty is a suspicious action that robots may perform under stress during the interview. Human suspects should avoid performing the penalty.</p>
        </div>
    );
}